using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity {
    public static class LayerUtil {

        public static void SetLayerRecursive(GameObject gao, int layerMask)
        {
            gao.layer = layerMask;
            foreach (Transform t in gao.transform) {
                SetLayerRecursive(t.gameObject, layerMask);
            }
        }
    }
}
