using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual.Deform {
    public class DeformBone {
        public Matrix mat;
        public Matrix invertedMat;
        public float unknown1;
        public ushort invert;
        public byte index;

        private Transform unityBone = null;
        public Transform UnityBone {
            get {
                if (unityBone == null) {
                    GameObject gao = new GameObject("Bone " + index + " - " + unknown1 + " - " + invert);
                    unityBone = gao.transform;
                    unityBone.localPosition = DefaultPosition;
                    unityBone.localRotation = DefaultRotation;
                    unityBone.localScale = DefaultScale;

                    // Visualization
                    MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    Mesh mesh = Util.CreateBox(0.1f);
                    mf.mesh = mesh;
                    mr.material = MapLoader.Loader.baseMaterial;
                }
                return unityBone;
            }
        }

        public Matrix TransformedMatrix {
            get {
                if (invertedMat == null) invertedMat = Matrix.Invert(mat);
                return invertedMat;
            }
        }
        public Vector3 DefaultPosition {
            get { return TransformedMatrix.GetPosition(convertAxes: true); }
        }
        public Quaternion DefaultRotation {
            get { return TransformedMatrix.GetRotation(convertAxes: true); }
        }
        public Vector3 DefaultScale {
            get { return TransformedMatrix.GetScale(convertAxes: true); }
        }

        // Call after clone
        public void Reset() {
            unityBone = null;
        }

        public DeformBone Clone() {
            DeformBone b = (DeformBone)MemberwiseClone();
            b.Reset();
            return b;
        }
    }
}
