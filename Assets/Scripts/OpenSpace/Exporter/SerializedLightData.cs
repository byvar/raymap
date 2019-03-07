using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Exporter {

    public class SerializedLightData {

        public Dictionary<string, LightInfo> Lights;

        public SerializedLightData(List<LightInfo> lights)
        {
            Lights = new Dictionary<string, LightInfo>();
            foreach (LightInfo l in lights) {
                Lights.Add(l.offset.ToString(), l);
            }
        }
    }
}
