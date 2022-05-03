using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class COL_Types_R2 : COL_Types {
		protected override void InitArrays() {
			ZDM = new COL_Type_ZDM[] {
				COL_Type_ZDM.Pieds,
				COL_Type_ZDM.Corps,
				COL_Type_ZDM.Tete,
				COL_Type_ZDM.Mains,
				COL_Type_ZDM.TraverseSol,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.Undefined,
				COL_Type_ZDM.CollisionneEau,
				COL_Type_ZDM.Undefined,
			};
			ZDE = new COL_Type_ZDE[] {
				COL_Type_ZDE.JaiMal,
				COL_Type_ZDE.JeFaisMal,
				COL_Type_ZDE.Undefined,
				COL_Type_ZDE.Grappin
			};
			ZDR = new COL_Type_ZDR[] {
				COL_Type_ZDR.Glissant,
				COL_Type_ZDR.Rebondissant,
				COL_Type_ZDR.Accrochage,
				COL_Type_ZDR.User1,
				COL_Type_ZDR.GrappinBis,
				COL_Type_ZDR.Gi,
				COL_Type_ZDR.Varappe,
				COL_Type_ZDR.Electrique,
				COL_Type_ZDR.Lave,
				COL_Type_ZDR.ChuteInfinie,
				COL_Type_ZDR.Mal,
				COL_Type_ZDR.Mort,
				COL_Type_ZDR.User2,
				COL_Type_ZDR.Undefined,
				COL_Type_ZDR.Eau,
				COL_Type_ZDR.NonCollisionnable
			};
		}
	}
}
