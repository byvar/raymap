using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    public class MorphMemory : MonoBehaviour {

        private Vector3[] originalVerts_ = null;

        public Vector3[] originalVerts {
            get
            {
                if (originalVerts_ == null) {
                    return null;
                }
                Vector3[] returnArray = new Vector3[originalVerts_.Length];
                Array.Copy(originalVerts_, returnArray, returnArray.Length);
                return returnArray;
            }
            set
            {
                originalVerts_ = value;
            }
        }

        public int objectIndex;
    }
}
