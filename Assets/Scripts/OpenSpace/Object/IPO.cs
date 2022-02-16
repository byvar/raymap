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
        public LegacyPointer offset;
        public LegacyPointer off_data;
        public LegacyPointer off_radiosity;
		public LegacyPointer off_portalCamera;

        public PhysicalObject data;
        public Radiosity radiosity;

        public string name = "";
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);
                    InitGameObject();
                }
                return gao;
            }
        }
        private void InitGameObject() {
            if (data != null) {
                /*if (ipo.data.visualSet != null) {
                    foreach (Visual.VisualSetLOD lod in ipo.data.visualSet) {
                        if (lod.obj.Gao != null) {
                            StaticBatchingUtility.Combine(lod.obj.Gao);
                        }
                    }
                }*/
            }
            /*System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            w.Start();*/
            if (data != null) data.Gao.transform.parent = Gao.transform;
            /*MapLoader.Loader.print("IPO: " + offset + " - " + w.ElapsedMilliseconds);
            w.Stop();*/
        }

        private SuperObject superObject;
        public SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public IPO(LegacyPointer offset, SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static IPO Read(Reader reader, LegacyPointer offset, SuperObject so) {
            MapLoader l = MapLoader.Loader;
            IPO ipo = new IPO(offset, so);
            ipo.off_data = LegacyPointer.Read(reader);
            ipo.off_radiosity = LegacyPointer.Read(reader);
			ipo.name = "IPO @ " + offset;

            // TODO: Read radiosity on all platforms. Currently crashes on Xbox 360
            //ipo.radiosity = l.FromOffsetOrRead<Radiosity>(reader, ipo.off_radiosity);

			if (Legacy_Settings.s.engineVersion >= Legacy_Settings.EngineVersion.R3) {
				reader.ReadUInt32();
				ipo.off_portalCamera = LegacyPointer.Read(reader);
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				if (Legacy_Settings.s.hasNames) ipo.name = reader.ReadString(0x32);
			}
			LegacyPointer.DoAt(ref reader, ipo.off_data, () => {
				ipo.data = PhysicalObject.Read(reader, ipo.off_data, so:ipo.superObject, radiosity: ipo.radiosity);
			});
			/*Pointer.DoAt(ref reader, ipo.off_portalCamera, () => {
				ipo.portalCamera = SuperObject.FromOffsetOrRead(reader, 
			});*/
            return ipo;
        }

    }
}
