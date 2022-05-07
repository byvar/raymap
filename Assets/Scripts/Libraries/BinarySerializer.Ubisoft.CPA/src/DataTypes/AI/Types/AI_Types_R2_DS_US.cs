using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_R2_DS_US : AI_Types_R2_DS_EU {

		#region Conditions
		protected override void InitConditions() {
			// Same list but with IsUSBuild added
			base.InitConditions();
			var ind = Array.IndexOf(Conditions, AI_Condition.IsCheatMenu);
			var condList = Conditions.ToList();
			condList.Insert(ind+1, AI_Condition.IsUSBuild);
			Conditions = condList.ToArray();
		}
		#endregion

	}

}
