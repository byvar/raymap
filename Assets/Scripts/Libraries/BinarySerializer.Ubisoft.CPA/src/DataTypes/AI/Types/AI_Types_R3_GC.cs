using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_R3_GC : AI_Types_R3_PS2 {
		protected override void InitProcedures() {
			base.InitProcedures();
			throw new NotImplementedException();
			//Procedures = InsertIntoArray(Procedures, 219, "Unknown_GamecubeOnly");
		}

		private string[] InsertIntoArray(string[] array, int index, string item) {
			var list = new List<string>(array);
			list.Insert(index, item);
			return list.ToArray();
		}
	}
}
