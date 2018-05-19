using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3DeformSet : IR3GeometricElement {
        public R3Mesh mesh;
        public R3Pointer offset;

        public R3Pointer off_weights;
        public R3Pointer off_bones;
        public uint num_weights;
        public byte num_bones;
        
        public R3DeformVertexWeights[] r3weights;
        public R3DeformBone[] r3bones;

        public BoneWeight[] weights;
        public Transform[] bones;
        public Matrix4x4[] bindPoses;


        public R3DeformSet(R3Pointer offset, R3Mesh mesh) {
            this.mesh = mesh;
            this.offset = offset;
        }

        public void InitUnityBones() {
            weights = new BoneWeight[mesh.num_vertices];
            for (int i = 0; i < mesh.num_vertices; i++) {
                weights[i] = new BoneWeight();
                weights[i].boneIndex0 = 0;
                weights[i].boneIndex1 = 0;
                weights[i].boneIndex2 = 0;
                weights[i].boneIndex3 = 0;
                weights[i].weight0 = 1f;
                weights[i].weight1 = 0;
                weights[i].weight2 = 0;
                weights[i].weight3 = 0;
            }
            for (int i = 0; i < num_weights; i++) {
                weights[r3weights[i].vertexIndex] = r3weights[i].UnityWeight;
            }
            bones = new Transform[num_bones];
            for (int i = 0; i < num_bones; i++) {
                bones[i] = r3bones[i].UnityBone;
            }
            bindPoses = new Matrix4x4[num_bones];
            for (int i = 0; i < num_bones; i++) {
                bindPoses[i] = bones[i].worldToLocalMatrix * mesh.gao.transform.localToWorldMatrix;
                //bindPoses[i] = r3bones[i].mat.m * bones[i].worldToLocalMatrix * mesh.gao.transform.localToWorldMatrix;
            }
        }
        

        public static R3DeformSet Read(EndianBinaryReader reader, R3Pointer offset, R3Mesh m) {
            R3Loader l = R3Loader.Loader;
            R3DeformSet d = new R3DeformSet(offset, m);
            d.off_weights = R3Pointer.Read(reader);
            d.off_bones = R3Pointer.Read(reader);
            d.num_weights = reader.ReadUInt16();
            d.num_bones = reader.ReadByte();
            d.num_bones += 1;
            // one more byte here, always zero? padding?

            // Create arrays
            d.r3bones = new R3DeformBone[d.num_bones]; // add root bone
            d.r3weights = new R3DeformVertexWeights[d.num_weights];

            // Read weights
            R3Pointer.Goto(ref reader, d.off_weights);
            for (int i = 0; i < d.num_weights; i++) {
                R3Pointer off_weightsForVertex = R3Pointer.Read(reader);
                uint vertex_index = reader.ReadUInt16();
                byte num_weightsForVertex = reader.ReadByte();
                reader.ReadByte(); // 0, padding
                d.r3weights[i] = new R3DeformVertexWeights(vertex_index);
                R3Pointer curPos = R3Pointer.Goto(ref reader, off_weightsForVertex);
                for (int j = 0; j < num_weightsForVertex; j++) {
                    uint weight = reader.ReadUInt16();
                    //float floatWeight = weight / UInt16.MaxValue;
                    byte boneIndex = reader.ReadByte();
                    reader.ReadByte(); // 0, padding
                    d.r3weights[i].boneWeights.Add(boneIndex, weight);
                }
                R3Pointer.Goto(ref reader, curPos);
            }

            // Read bones
            d.r3bones[0] = new R3DeformBone();
            d.r3bones[0].mat = new R3Matrix(null, 1, Matrix4x4.identity, new Vector4(1f, 1f, 1f, 1f));
            d.r3bones[0].index = 0;
            R3Pointer.Goto(ref reader, d.off_bones);
            for (int i = 1; i < d.num_bones; i++) {
                d.r3bones[i] = new R3DeformBone();

                // each bone is a 0x38 block
                Matrix4x4 mat = new Matrix4x4();
                mat.SetColumn(3,new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1f));
                for (int j = 0; j < 3; j++) {
                    mat.SetColumn(j, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1f));
                }
                d.r3bones[i].mat = new R3Matrix(null, 1, mat, new Vector4(1f, 1f, 1f, 1f));
                d.r3bones[i].unknown1 = reader.ReadSingle();
                d.r3bones[i].unknown2 = reader.ReadUInt16();
                d.r3bones[i].index = reader.ReadByte();
                reader.ReadByte(); // 0, padding
            }
            d.InitUnityBones();

            return d;
        }

        public IR3GeometricElement Clone(R3Mesh mesh) {
            R3DeformSet d = (R3DeformSet)MemberwiseClone();
            d.mesh = mesh;
            d.r3bones = new R3DeformBone[r3bones.Length];
            for (int i = 0; i < r3bones.Length; i++) {
                d.r3bones[i] = r3bones[i].Clone();
            }
            d.r3weights = new R3DeformVertexWeights[r3weights.Length];
            for (int i = 0; i < r3weights.Length; i++) {
                d.r3weights[i] = r3weights[i].Clone();
            }
            d.InitUnityBones();
            return d;
        }
    }

    public class R3DeformVertexWeights {
        public uint vertexIndex;
        public Dictionary<byte, uint> boneWeights;

        public R3DeformVertexWeights(uint vertexIndex) {
            this.vertexIndex = vertexIndex;
            boneWeights = new Dictionary<byte, uint>();
        }

        private bool unityWeightCalculated = false;
        private BoneWeight unityWeight;
        public BoneWeight UnityWeight {
            get {
                if (!unityWeightCalculated) {
                    unityWeight = new BoneWeight();
                    unityWeight.boneIndex0 = 0;
                    unityWeight.boneIndex1 = 0;
                    unityWeight.boneIndex2 = 0;
                    unityWeight.boneIndex3 = 0;
                    unityWeight.weight0 = 0;
                    unityWeight.weight1 = 0;
                    unityWeight.weight2 = 0;
                    unityWeight.weight3 = 0;
                    List<KeyValuePair<byte, uint>> sortedWeights = boneWeights.OrderByDescending(w => w.Value).ToList();
                    UInt16 sortedWeightsSum = (UInt16)sortedWeights.Select(w => (Int32)(w.Value)).Sum();
                    if (sortedWeightsSum < UInt16.MaxValue) {
                        sortedWeights.Add(new KeyValuePair<byte, uint>(0, (uint)(UInt16.MaxValue - sortedWeightsSum)));
                    }
                    sortedWeights = sortedWeights.OrderByDescending(w => w.Value).ToList();
                    if (sortedWeights.Count > 0) {
                        unityWeight.boneIndex0 = sortedWeights[0].Key;
                        unityWeight.weight0 = (float)sortedWeights[0].Value / (float)UInt16.MaxValue;
                    }
                    if (sortedWeights.Count > 1) {
                        unityWeight.boneIndex1 = sortedWeights[1].Key;
                        unityWeight.weight1 = (float)sortedWeights[1].Value / (float)UInt16.MaxValue;
                    }
                    if (sortedWeights.Count > 2) {
                        unityWeight.boneIndex2 = sortedWeights[2].Key;
                        unityWeight.weight2 = (float)sortedWeights[2].Value / (float)UInt16.MaxValue;
                    }
                    if (sortedWeights.Count > 3) {
                        unityWeight.boneIndex3 = sortedWeights[3].Key;
                        unityWeight.weight3 = (float)sortedWeights[3].Value / (float)UInt16.MaxValue;
                    }
                    if (sortedWeights.Count > 4) {
                        R3Loader.Loader.print("Unity does not support more than 4 bones affecting a vertex at once.");
                    }
                    unityWeightCalculated = true;
                }
                return unityWeight;
            }
        }

        // Call after clone
        public void Reset() {
            unityWeightCalculated = false;
        }

        public R3DeformVertexWeights Clone() {
            R3DeformVertexWeights w = (R3DeformVertexWeights)MemberwiseClone();
            w.Reset();
            return w;
        }
    }

    public class R3DeformBone {
        public R3Matrix mat;
        public float unknown1;
        public uint unknown2;
        public byte index;

        private Transform unityBone = null;
        public Transform UnityBone {
            get {
                if (unityBone == null) {
                    GameObject gao = new GameObject("Bone " + index + " - " + unknown1 + " - " + unknown2);
                    unityBone = gao.transform;
                    unityBone.localPosition = mat.GetPosition(convertAxes: true);
                    unityBone.localRotation = mat.GetRotation(convertAxes: true);
                    unityBone.localScale = mat.GetScale(convertAxes: true);
                }
                return unityBone;
            }
        }

        // Call after clone
        public void Reset() {
            unityBone = null;
        }

        public R3DeformBone Clone() {
            R3DeformBone b = (R3DeformBone)MemberwiseClone();
            b.Reset();
            return b;
        }
    }
}
