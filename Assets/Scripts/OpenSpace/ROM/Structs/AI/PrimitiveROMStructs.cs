using OpenSpace.ROM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.ROM {

   public class ROMReal : ROMStruct {
        public float value;
        protected override void ReadInternal(Reader reader) {
            value = reader.ReadSingle();
        }
    }

    public class ROMInt32 : ROMStruct {
        public int value;
        protected override void ReadInternal(Reader reader)  {
            value = reader.ReadInt32();
        }
	}

	public class ROMVector3 : ROMStruct {
		public Vector3 value;
		protected override void ReadInternal(Reader reader) {
			value = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
	}
}
