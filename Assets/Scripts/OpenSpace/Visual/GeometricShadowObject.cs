using OpenSpace.Object;
using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Geometric Shadow Object. It's only used in Dinosaur
    /// </summary>
    public class GeometricShadowObject : IEngineObject {
        public Pointer offset;
        public Pointer off_data;
        public GeometricObject data;
        public uint unk4;
        public uint unk8;
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("Shadow @ " + offset);
                }
                return gao;
            }
        }

        private SuperObject superObject;
        public SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public GeometricShadowObject(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static GeometricShadowObject Read(Reader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            GeometricShadowObject igo = new GeometricShadowObject(offset, so);
            igo.off_data = Pointer.Read(reader);
            igo.unk4 = reader.ReadUInt32();
            igo.unk8 = reader.ReadUInt32();

			Pointer.DoAt(ref reader, igo.off_data, () => {
                igo.data = GeometricObject.Read(reader, igo.off_data);
				if (igo.data != null) {
					igo.data.Gao.transform.parent = igo.Gao.transform;
                    igo.Gao.name = "[Shadow] " + igo.data.Gao.name;
                    igo.Gao.SetActive(false); // Shadows don't draw well right now
				}
			});
            return igo;
        }

    }
}
