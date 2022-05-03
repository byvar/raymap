using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_AIModel : U64_Struct {
		public U64_Reference<U64_Placeholder> VariableDeclaration { get; set; }
		public U64_Reference<AI_Intelligence> Intelligence { get; set; }
		public U64_Reference<AI_Intelligence> Reflex { get; set; }

		// For FastC
		public U64_Reference<AI_Intelligence> Reference { get; set; }
		public U64_ArrayReference<AI_NodeInterpret> ReferenceTree { get; set; } // Full NodeInterpret reference!
		public ushort ReferenceTreeSize { get; set; }
		public short CFast_FunctionIndex { get; set; }
		public bool CFast_IsC { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VariableDeclaration = s.SerializeObject<U64_Reference<U64_Placeholder>>(VariableDeclaration, name: nameof(VariableDeclaration));
			Intelligence = s.SerializeObject<U64_Reference<AI_Intelligence>>(Intelligence, name: nameof(Intelligence))?.Resolve(s);
			Reflex = s.SerializeObject<U64_Reference<AI_Intelligence>>(Reflex, name: nameof(Reflex))?.Resolve(s);

			// For FastC
			Reference = s.SerializeObject<U64_Reference<AI_Intelligence>>(Reference, name: nameof(Reference));
			ReferenceTree = s.SerializeObject<U64_ArrayReference<AI_NodeInterpret>>(ReferenceTree, name: nameof(ReferenceTree));
			ReferenceTreeSize = s.Serialize<ushort>(ReferenceTreeSize, name: nameof(ReferenceTreeSize));
			CFast_FunctionIndex = s.Serialize<short>(CFast_FunctionIndex, name: nameof(CFast_FunctionIndex));
			s.DoBits<ushort>(b => {
				CFast_IsC = b.SerializeBits<bool>(CFast_IsC, 1, name: nameof(CFast_IsC));
			});


			if (CFast_IsC && !Reference.IsNull) {
				Reference?.Resolve(s);
				ReferenceTree?.Resolve(s, ReferenceTreeSize / AI_NodeInterpret.StructSize);
			}
		}
	}
}
