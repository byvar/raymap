using System;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_RelocationTableBlock : BinarySerializable {
		public byte Module { get; set; }
		public byte Block { get; set; }
		public uint Count { get; set; }

		public SNA_RelocationTablePointer[] Pointers { get; set; }

		public SNA_Module? ModuleTranslation {
			get => Context.GetCPASettings().SNATypes?.GetModule(Module);
			set {
				if (!value.HasValue) return;
				var val = value.Value;
				var newModule = Context.GetCPASettings().SNATypes?.GetModuleInt(val);
				if (newModule.HasValue) Module = (byte)newModule.Value;
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			Module = s.Serialize<byte>(Module, name: nameof(Module));
			if (ModuleTranslation != null) s.Log($"Module: {ModuleTranslation}");

			Block = s.Serialize<byte>(Block, name: nameof(Block));
			Count = s.Serialize<uint>(Count, name: nameof(Count));

			s.DoEncoded(SNA_LZOEncoder.GetIfRequired(s.GetCPASettings(), Count), () => {
				Pointers = s.SerializeObjectArray<SNA_RelocationTablePointer>(Pointers, Count, name: nameof(Pointers));
			});
		}
	}
}
