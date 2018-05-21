using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3DeformBone {
        public R3Matrix mat;
        public float unknown1;
        public ushort unknown2;
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
