using OpenSpace.ROM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.ROM {

   public class Scr_Real : ROMStruct {
        public float value;
        protected override void ReadInternal(Reader reader) {
            value = reader.ReadSingle();
        }
    }

    public class Scr_Int : ROMStruct {
        public int value;
        protected override void ReadInternal(Reader reader)  {
            value = reader.ReadInt32();
        }
	}

	public class Scr_Vector3 : ROMStruct {
		public Vector3 value;
		protected override void ReadInternal(Reader reader) {
			value = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
	}

	public class Scr_String : ROMStruct {
		public Reference<String> str;
		public ushort sz_str;

		protected override void ReadInternal(Reader reader) {
			//R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			sz_str = reader.ReadUInt16();
			str = new Reference<String>(reader, resolve: true, onPreRead: (s) => { s.length = sz_str; });
			//Debug.Log("Loaded string " + str.Value.str);
		}

		public override string ToString() {
			return "\"" + str.Value.str + "\"";
		}
	}

	public class Dsg_Real : ROMStruct {
		public float value;
		protected override void ReadInternal(Reader reader) {
			value = reader.ReadSingle();
		}
	}

	public class Dsg_Int : ROMStruct {
		public int value;
		protected override void ReadInternal(Reader reader) {
			value = reader.ReadInt32();
		}
	}

	public class Dsg_UInt : ROMStruct {
		public uint value;
		protected override void ReadInternal(Reader reader) {
			value = reader.ReadUInt32();
		}
	}

	public class Dsg_Vector3 : ROMStruct {
		public Vector3 value;
		protected override void ReadInternal(Reader reader) {
			value = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
	}
}
