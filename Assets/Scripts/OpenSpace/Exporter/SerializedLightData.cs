using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Exporter {

    public class SerializedLightData {

        public Dictionary<string, ELight> Lights;

        public SerializedLightData(List<LightInfo> lights)
        {
            Lights = new Dictionary<string, ELight>();
            foreach (LightInfo l in lights) {
                Lights.Add(l.offset.ToString(), new ELight(l));
            }
        }

        public class ELight {
            public LightInfo LightInfo;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;

            public ELight(LightInfo info)
            {
                this.LightInfo = info;
                position = info.transMatrix.GetPosition();
                rotation = info.transMatrix.GetRotation().eulerAngles;
                scale = info.transMatrix.GetScale();
            }
        }
    }
}
