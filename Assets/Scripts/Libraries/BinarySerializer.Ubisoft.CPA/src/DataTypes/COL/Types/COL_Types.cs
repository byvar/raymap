using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class COL_Types {
		public COL_Type_ZDE[] ZDE { get; protected set; }
		public COL_Type_ZDM[] ZDM { get; protected set; }
		public COL_Type_ZDR[] ZDR { get; protected set; }

		// TODO: Add identifier names like MAX_VARAPPE
		public void Init() {
			InitArrays();
			CreateDictionaries();
		}

		protected abstract void InitArrays();

		private void CreateDictionaries() {
			// TODO
		}

		public COL_Type_ZDE GetType_ZDE(int type) {
			if (type < ZDE.Length) return ZDE[type];
			return COL_Type_ZDE.Undefined;
		}
		public COL_Type_ZDM GetType_ZDM(int type) {
			if (type < ZDM.Length) return ZDM[type];
			return COL_Type_ZDM.Undefined;
		}
		public COL_Type_ZDR GetType_ZDR(int type) {
			if (type < ZDR.Length) return ZDR[type];
			return COL_Type_ZDR.Undefined;
		}
	}
}
