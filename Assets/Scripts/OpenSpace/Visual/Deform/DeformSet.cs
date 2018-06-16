using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual.Deform {
    public class DeformSet : IGeometricElement {
        public MeshObject mesh;
        public Pointer offset;

        public Pointer off_weights;
        public Pointer off_bones;
        public ushort num_weights;
        public byte num_bones;
        
        public DeformVertexWeights[] r3weights;
        public DeformBone[] r3bones;

        public BoneWeight[] weights;
        public Transform[] bones;
        public Matrix4x4[] bindPoses;


        public DeformSet(Pointer offset, MeshObject mesh) {
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
        

        public static DeformSet Read(EndianBinaryReader reader, Pointer offset, MeshObject m) {
            MapLoader l = MapLoader.Loader;
            DeformSet d = new DeformSet(offset, m);
            d.off_weights = Pointer.Read(reader);
            d.off_bones = Pointer.Read(reader);
            d.num_weights = reader.ReadUInt16();
            d.num_bones = reader.ReadByte();
            d.num_bones += 1;
            // one more byte here, always zero? padding?

            // Create arrays
            d.r3bones = new DeformBone[d.num_bones]; // add root bone
            d.r3weights = new DeformVertexWeights[d.num_weights];

            // Read weights
            Pointer.Goto(ref reader, d.off_weights);
            for (int i = 0; i < d.num_weights; i++) {
                Pointer off_weightsForVertex = Pointer.Read(reader);
                ushort vertex_index = reader.ReadUInt16();
                byte num_weightsForVertex = reader.ReadByte();
                reader.ReadByte(); // 0, padding
                d.r3weights[i] = new DeformVertexWeights(vertex_index);
                Pointer curPos = Pointer.Goto(ref reader, off_weightsForVertex);
                for (int j = 0; j < num_weightsForVertex; j++) {
                    ushort weight = reader.ReadUInt16();
                    //float floatWeight = weight / UInt16.MaxValue;
                    byte boneIndex = reader.ReadByte();
                    reader.ReadByte(); // 0, padding
                    d.r3weights[i].boneWeights.Add(boneIndex, weight);
                }
                Pointer.Goto(ref reader, curPos);
            }

            // Read bones
            d.r3bones[0] = new DeformBone();
            d.r3bones[0].mat = new Matrix(null, 1, Matrix4x4.identity, new Vector4(1f, 1f, 1f, 1f));
            d.r3bones[0].index = 0xFF;
            Pointer.Goto(ref reader, d.off_bones);
            for (int i = 1; i < d.num_bones; i++) {
                d.r3bones[i] = new DeformBone();

                // each bone is a 0x38 block
                Matrix4x4 mat = new Matrix4x4();
                mat.SetColumn(3,new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1f));
                for (int j = 0; j < 3; j++) {
                    mat.SetColumn(j, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1f));
                }
                d.r3bones[i].mat = new Matrix(null, 1, mat, new Vector4(1f, 1f, 1f, 1f));
                d.r3bones[i].unknown1 = reader.ReadSingle();
                d.r3bones[i].unknown2 = reader.ReadUInt16();
                d.r3bones[i].index = reader.ReadByte();
                reader.ReadByte(); // 0, padding
            }
            d.InitUnityBones();

            return d;
        }

        public IGeometricElement Clone(MeshObject mesh) {
            DeformSet d = (DeformSet)MemberwiseClone();
            d.mesh = mesh;
            d.r3bones = new DeformBone[r3bones.Length];
            for (int i = 0; i < r3bones.Length; i++) {
                d.r3bones[i] = r3bones[i].Clone();
            }
            d.r3weights = new DeformVertexWeights[r3weights.Length];
            for (int i = 0; i < r3weights.Length; i++) {
                d.r3weights[i] = r3weights[i].Clone();
            }
            d.InitUnityBones();
            return d;
        }
    }
}
