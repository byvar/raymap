using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_TripledIndex : U64_Struct {
		public ushort Index0 { get; set; }
		public ushort Index1 { get; set; }
		public ushort Index2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index0 = s.Serialize<ushort>(Index0, name: nameof(Index0));
			Index1 = s.Serialize<ushort>(Index1, name: nameof(Index1));
			Index2 = s.Serialize<ushort>(Index2, name: nameof(Index2));
		}
		public override string ShortLog => $"TripledIndex({Index0:X4}, {Index1:X4}, {Index2:X4})";
		public override bool UseShortLog => true;
	}
}
