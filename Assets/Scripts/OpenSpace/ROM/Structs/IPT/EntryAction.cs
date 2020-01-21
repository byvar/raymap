using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	// Size: 6
	public class EntryAction : ROMStruct {
		public Reference<KeyWordArray> keywords;
		public ushort num_keywords;
		public ushort unk;

        protected override void ReadInternal(Reader reader) {
			keywords = new Reference<KeyWordArray>(reader);
			num_keywords = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			if (num_keywords > 0) {
				keywords.Resolve(reader, a => a.length = num_keywords);
				Loader.print(ToString());
			}
        }

		public override string ToString() {
			if (num_keywords > 0 && keywords.Value != null) {
				return "EntryAction_" + string.Format("{0:X4}", Index) + " = EntryAction(" + keywords.Value.keywords[0].ToString() + ")";
			} else {
				return "EntryAction_" + string.Format("{0:X4}", Index) + " = EntryAction.Empty";
			}
		}
	}
}
