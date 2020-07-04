using OpenSpace.Visual.ISI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object {
    /// <summary>
    /// IPO = Instantiated Physical Object. Used for level geometry
    /// </summary>
    public class IPO : IEngineObject {
        public Pointer offset;
        public Pointer off_data;
        public Pointer off_radiosity;
		public Pointer off_portalCamera;

        public PhysicalObject data;
        public Radiosity radiosity;

        public string name = "";
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);
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

        public IPO(Pointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static IPO Read(Reader reader, Pointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            IPO ipo = new IPO(offset, so);
            ipo.off_data = Pointer.Read(reader);
            ipo.off_radiosity = Pointer.Read(reader);
			ipo.name = "IPO @ " + offset;

            // TODO: Read radiosity on all platforms. Currently crashes on Xbox 360
            //ipo.radiosity = l.FromOffsetOrRead<Radiosity>(reader, ipo.off_radiosity);

			if (Settings.s.engineVersion >= Settings.EngineVersion.R3) {
				reader.ReadUInt32();
				ipo.off_portalCamera = Pointer.Read(reader);
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				if (Settings.s.hasNames) ipo.name = reader.ReadString(0x32);
			}
			Pointer.DoAt(ref reader, ipo.off_data, () => {
				ipo.data = PhysicalObject.Read(reader, ipo.off_data, radiosity: ipo.radiosity);
				if (ipo.data != null) {
					/*if (ipo.data.visualSet != null) {
						foreach (Visual.VisualSetLOD lod in ipo.data.visualSet) {
							if (lod.obj.Gao != null) {
								StaticBatchingUtility.Combine(lod.obj.Gao);
							}
						}
					}*/
					ipo.data.Gao.transform.parent = ipo.Gao.transform;
				}
			});
			/*Pointer.DoAt(ref reader, ipo.off_portalCamera, () => {
				ipo.portalCamera = SuperObject.FromOffsetOrRead(reader, 
			});*/
            /*if (ipo.data != null && ipo.data.visualSet.Count > 0) {
                if (ipo.data.visualSet[0].obj is R3Mesh) {
                    GameObject meshGAO = ((R3Mesh)ipo.data.visualSet[0].obj).gao;
                    meshGAO.transform.parent = ipo.Gao.transform;
                }
            }*/
            return ipo;
        }

    }
}
