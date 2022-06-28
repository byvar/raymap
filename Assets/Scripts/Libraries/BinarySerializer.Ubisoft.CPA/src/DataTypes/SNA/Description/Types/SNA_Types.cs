using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class SNA_Types {
		public Dictionary<int, SNA_DescriptionType> DSBTypes { get; protected set; }
		public Dictionary<SNA_DescriptionType, int> DSBTypes_Reverse { get; protected set; }

		public void Init() {
			InitArrays();
			CreateDictionaries();
		}

		protected abstract void InitArrays();

		private void CreateDictionaries() {
			DSBTypes_Reverse = new Dictionary<SNA_DescriptionType, int>();
			foreach (var kv in DSBTypes) {
				DSBTypes_Reverse[kv.Value] = kv.Key;
			}
		}

		public SNA_DescriptionType GetType(int type) => DSBTypes[type];
		public int GetInt(SNA_DescriptionType type) => DSBTypes_Reverse[type];
	}
}
