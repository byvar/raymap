using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSpace.Waypoints {
    public class WayPoint {

        public Pointer offset;
        public Vector3 position;

        public List<GraphNode> containingGraphNodes;

        // For isolate waypoints
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) InitGameObject();
                return gao;
            }
        }
        private void InitGameObject() {
            gao = new GameObject("WayPoint");
            gao.transform.position = new Vector3(position.x, position.z, position.y);
            WaypointBehaviour wpBehaviour = gao.AddComponent<WaypointBehaviour>();
        }
        // ^ for isolate waypoints

        public WayPoint(Pointer offset) {
            this.offset = offset;
            containingGraphNodes = new List<GraphNode>();
        }

        public static WayPoint FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.waypoints.FirstOrDefault(w => w.offset == offset);
        }

        public static WayPoint FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            WayPoint w = FromOffset(offset);
            if (w == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    w = WayPoint.Read(reader, offset);
                    MapLoader.Loader.waypoints.Add(w);
                });
            }
            return w;
        }

        public static WayPoint Read(Reader reader, Pointer offset) {
            WayPoint wp = new WayPoint(offset);

            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            wp.position = new Vector3(x, y, z);

            return wp;
        }
    }
}