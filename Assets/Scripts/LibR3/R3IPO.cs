using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// IPO = Instanciated Physical Object. Used for level geometry
    /// </summary>
    public class R3IPO : IR3Data {
        public R3Pointer offset;
        public R3Pointer off_data;
        public R3Pointer off_radiosity;
        public R3PhysicalObject data;
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

        private R3SuperObject superObject;
        public R3SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public R3IPO(R3Pointer offset, R3SuperObject so) {
            this.offset = offset;
            this.superObject = so;
        }

        public static R3IPO Read(EndianBinaryReader reader, R3Pointer offset, R3SuperObject so) {
            R3Loader l = R3Loader.Loader;
            R3IPO ipo = new R3IPO(offset, so);
            ipo.off_data = R3Pointer.Read(reader);
            ipo.off_radiosity = R3Pointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            ipo.name = "IPO";
            if (l.mode == R3Loader.Mode.Rayman3GC) ipo.name = new string(reader.ReadChars(0x32));
            R3Pointer.Goto(ref reader, ipo.off_data);
            ipo.data = R3PhysicalObject.Read(reader, ipo.off_data);
            if (ipo.data != null) {
                if (ipo.data is R3Mesh) {
                    GameObject meshGAO = ((R3Mesh)ipo.data).gao;
                    meshGAO.transform.parent = ipo.Gao.transform;
                }
            }
            return ipo;
        }

    }
}
