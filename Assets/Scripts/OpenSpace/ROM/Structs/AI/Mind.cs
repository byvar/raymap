using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Mind : ROMStruct {
		// Size: 4
		public Reference<AIModel> aiModel;
        public ushort intelligenceNormal;
        public ushort intelligenceReflex;
        //public Reference;

        protected override void ReadInternal(Reader reader) {
			aiModel = new Reference<AIModel>(reader, true); //0
            intelligenceNormal = reader.ReadUInt16();
            intelligenceReflex = reader.ReadUInt16();
        }
	}
}
