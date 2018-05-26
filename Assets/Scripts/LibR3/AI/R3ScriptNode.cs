using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3ScriptNode {
        public R3Pointer offset;
        public uint param;
        public byte type;
        public byte indent;

        public R3ScriptNode(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3ScriptNode Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            R3ScriptNode sn = new R3ScriptNode(offset);

            sn.param = reader.ReadUInt32();

            // Could be read as one uint, but the code says that type is only 1 byte
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            sn.type = reader.ReadByte();

            reader.ReadByte();
            reader.ReadByte();
            sn.indent = reader.ReadByte();
            reader.ReadByte();
            /*if (sn.indent == 0) {
                l.print("---- END OF SCRIPT ----");
            } else {
                l.print(new String(' ', sn.indent - 1) + R3AIFunctionTypes.readableFunctionSubType(reader, sn.type, (int)sn.param));
            }*/
            return sn;
        }
    }
}
