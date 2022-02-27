using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_LevelsNameList : U64_Struct {
		public U64_Reference<U64_Placeholder> Level { get; set; }
		public string Name { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Level = s.SerializeObject<U64_Reference<U64_Placeholder>>(Level, name: nameof(Level));
			Name = s.SerializeString(Name, 62, name: nameof(Name));
		}
	}
}
