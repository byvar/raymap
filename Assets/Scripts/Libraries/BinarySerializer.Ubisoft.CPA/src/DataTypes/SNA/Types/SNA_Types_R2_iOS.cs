using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Types_R2_iOS : SNA_Types_R2 {
		protected override void InitArrays() {
			base.InitArrays();

			Modules = Modules.Except(new SNA_Module[] {
				SNA_Module.PO
			}).ToArray();
		}
	}
}
