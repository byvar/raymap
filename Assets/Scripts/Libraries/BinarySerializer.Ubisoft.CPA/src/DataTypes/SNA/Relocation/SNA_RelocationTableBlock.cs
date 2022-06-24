using System;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_RelocationTableBlock : BinarySerializable {
		public byte Module { get; set; }
		public byte Block { get; set; }
		public uint Count { get; set; }

		public SNA_RelocationTablePointer[] Pointers { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			Module = s.Serialize<byte>(Module, name: nameof(Module));
			Block = s.Serialize<byte>(Block, name: nameof(Block));
			Count = s.Serialize<uint>(Count, name: nameof(Count));

			s.DoEncoded(SNA_LZOEncoder.GetIfRequired(s.GetCPASettings()), () => {
				Pointers = s.SerializeObjectArray<SNA_RelocationTablePointer>(Pointers, Count, name: nameof(Pointers));
			});
		}
	}
}
