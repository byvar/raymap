using OpenSpace.Loader;
using System.Linq;
using UnityEngine;
using DsgVarType = OpenSpace.AI.DsgVarInfoEntry.DsgVarType;

namespace OpenSpace.ROM {
	public class DsgMemInfo : ROMStruct {
		// size: 6
		public ushort index_of_info;
		public ushort type;

		// Custom
		public DsgVarValue value;

		protected override void ReadInternal(Reader reader) {
			index_of_info = reader.ReadUInt16();
			type = reader.ReadUInt16();
			value = new DsgVarValue(index_of_info, type);
			value.Read(reader);
		}
	}
}
