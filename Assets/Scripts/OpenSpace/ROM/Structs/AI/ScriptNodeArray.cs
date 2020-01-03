using OpenSpace.Loader;
using OpenSpace.ROM.RSP;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ScriptNodeArray : ROMStruct {
		public ushort length;
		public ScriptNode[] nodes;


		protected override void ReadInternal(Reader reader) {
			Loader.print("Script @ " + Pointer.Current(reader) + " - len: " + length);
			nodes = new ScriptNode[length];
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i] = new ScriptNode(reader);
			}
		}


		public class ScriptNode {
			// size: 4
			public byte type;
			public byte indent;
			public ushort param;
			public ScriptNode(Reader reader) {
				type = reader.ReadByte();
				indent = reader.ReadByte();
				param = reader.ReadUInt16();
			}
		}
	}
}
