using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Brain : ROMStruct {
		// Size: 8
        public Reference<AIModel> aiModel;
		public Reference<Comport> comportIntelligence;
		public Reference<Comport> comportReflex;
		public Reference<DsgMem> dsgMem;

		protected override void ReadInternal(Reader reader) {
			aiModel = new Reference<AIModel>(reader, true);
			comportIntelligence = new Reference<Comport>(reader, true);
			comportReflex = new Reference<Comport>(reader, true);
			dsgMem = new Reference<DsgMem>(reader, true);
			if (dsgMem.Value != null && dsgMem.Value.num_info > 0) {
				for (int i = 0; i < dsgMem.Value.info.Value.info.Length; i++) {
					DsgMemInfo info = dsgMem.Value.info.Value.info[i].Value;
					DsgVarInfo.Entry entry = aiModel.Value.dsgVar.Value.info.Value.GetEntryFromIndex(info.value.index);
					Loader.print("DsgMemInfo "
						+ info.value.dsgVarType + (info.value.paramEntry?.Value != null ? "[" + info.value.paramEntry.Value.index_in_array + "]" : "")
						+ " - "
						+ entry.value.dsgVarType + (entry.value.paramEntry?.Value != null ? "[" + entry.value.paramEntry.Value.index_in_array + "]" : "")
						+ " - " + info.value.param + " - " + entry.value.param);
				}
			}
		}
	}
}
