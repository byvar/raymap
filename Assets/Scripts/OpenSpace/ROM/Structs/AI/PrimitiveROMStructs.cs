using OpenSpace.ROM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {

   public class FloatROMStruct : ROMStruct {

        public float value;

        protected override void ReadInternal(Reader reader)
        {
            value = reader.ReadSingle();
        }
    }

    public class Int32ROMStruct : ROMStruct {

        public int value;

        protected override void ReadInternal(Reader reader)
        {
            value = reader.ReadInt32();
        }
    }
}
