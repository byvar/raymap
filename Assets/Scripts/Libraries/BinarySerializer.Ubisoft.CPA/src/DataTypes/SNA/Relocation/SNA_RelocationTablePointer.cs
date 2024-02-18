using System;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_RelocationTablePointer : BinarySerializable {
		public uint Pointer { get; set; } // Absolute pointer to an offset in the block determined by TargetModule & TargetBlock
		public byte TargetModule { get; set; }
		public byte TargetBlock { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Pointer = s.Serialize<uint>(Pointer, name: nameof(Pointer));
			TargetModule = s.Serialize<byte>(TargetModule, name: nameof(TargetModule));
			TargetBlock = s.Serialize<byte>(TargetBlock, name: nameof(TargetBlock));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)
				|| (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.PlaymobilLauraPentiumIII))) {
				s.SerializePadding(2); // 0xCD 0xCD
			}
		}
	}
}
