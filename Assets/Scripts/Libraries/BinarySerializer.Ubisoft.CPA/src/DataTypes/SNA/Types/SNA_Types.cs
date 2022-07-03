using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class SNA_Types {
		protected Dictionary<int, SNA_DescriptionType> DSBTypes { get; set; }
		protected Dictionary<SNA_DescriptionType, int> DSBTypes_Reverse { get; set; }
		protected SNA_Module[] Modules { get; set; }

		public void Init() {
			InitArrays();
			CreateDictionaries();
		}

		protected abstract void InitArrays();

		private void CreateDictionaries() {
			if (DSBTypes != null) {
				DSBTypes_Reverse = new Dictionary<SNA_DescriptionType, int>();
				foreach (var kv in DSBTypes) {
					DSBTypes_Reverse[kv.Value] = kv.Key;
				}
			}
		}

		public SNA_DescriptionType GetDescriptionType(int type) => DSBTypes[type];
		public int GetDescriptionInt(SNA_DescriptionType type) => DSBTypes_Reverse[type];
		public SNA_Module? GetModule(int index) {
			if(index == 0xFF) return SNA_Module.Unassigned;
			if(Modules == null || Modules.Length <= index) return null;
			return Modules[index];
		}
		public int? GetModuleInt(SNA_Module module) {
			if(module == SNA_Module.Unassigned) return 0xFF;
			if(Modules == null) return null;
			if(Modules.Contains(module))
				return System.Array.IndexOf(Modules, module);
			return null;
		}
	}
}
