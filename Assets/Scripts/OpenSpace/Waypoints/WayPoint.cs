using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSpace.Waypoints {
    public class WayPoint : IReferenceable{

        public LegacyPointer offset;
        public Vector3 position;
        public float radius;

        public LegacyPointer off_perso_so;

        [JsonIgnore]
        public List<GraphNode> containingGraphNodes;

        [JsonIgnore]
        public ReferenceFields References { get; set; } = new ReferenceFields();

        // For isolate waypoints
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) InitGameObject();
                return gao;
            }
        }
        private void InitGameObject() {
            gao = new GameObject("WayPoint ("+this.offset+")");
            gao.transform.position = new Vector3(position.x, position.z, position.y);
            WayPointBehaviour wpBehaviour = gao.AddComponent<WayPointBehaviour>();
            gao.layer = LayerMask.NameToLayer("Graph");
            wpBehaviour.wp = this;
            wpBehaviour.radius = radius;
        }
        // ^ for isolate waypoints

        public WayPoint(LegacyPointer offset) {
            this.offset = offset;
            containingGraphNodes = new List<GraphNode>();
        }

        public static WayPoint FromOffset(LegacyPointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.waypoints.FirstOrDefault(w => w.offset == offset);
        }

        public static WayPoint FromOffsetOrRead(LegacyPointer offset, Reader reader) {
            if (offset == null) return null;
            WayPoint w = FromOffset(offset);
            if (w == null) {
                LegacyPointer.DoAt(ref reader, offset, () => {
                    w = WayPoint.Read(reader, offset);
                    MapLoader.Loader.waypoints.Add(w);
                });
            }
            return w;
        }

        public static WayPoint Read(Reader reader, LegacyPointer offset) {

            WayPoint wp = new WayPoint(offset);
            float radius = 0;
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                reader.ReadUInt32();
            }
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.TT) {
                radius = reader.ReadSingle();
            }

            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();

            if (CPA_Settings.s.engineVersion != CPA_Settings.EngineVersion.TT) {
                radius = reader.ReadSingle();
            }
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                wp.off_perso_so = LegacyPointer.Read(reader);// perso
            }

            wp.position = new Vector3(x, y, z);
            wp.radius = radius;

            return wp;
        }

        public class WayPointReferenceJsonConverter : JsonConverter {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(WayPoint);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                WayPoint wp = (WayPoint)value;
                
                var jt = JToken.FromObject(wp.offset.ToString());
                jt.WriteTo(writer);
            }
        }
    }
}