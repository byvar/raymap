using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_Character3dData : U64_Struct {
		public GLI_DrawMask DrawMask { get; set; }
		public MTH2D_Vector ShadowScale { get; set; }
		public U64_Reference<GAM_State> InitialState { get; set; }
		public U64_Reference<GAM_ObjectsTable> InitialObjectsTable { get; set; }
		public byte BrainComputationFrequency { get; set; }
		public bool IsAlphabet { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DrawMask = s.Serialize<GLI_DrawMask>(DrawMask, name: nameof(DrawMask));
			ShadowScale = s.SerializeObject<MTH2D_Vector>(ShadowScale, name: nameof(ShadowScale));
			InitialState = s.SerializeObject<U64_Reference<GAM_State>>(InitialState, name: nameof(InitialState))?.Resolve(s);
			InitialObjectsTable = s.SerializeObject<U64_Reference<GAM_ObjectsTable>>(InitialObjectsTable, name: nameof(InitialObjectsTable))?.Resolve(s);
			BrainComputationFrequency = s.Serialize<byte>(BrainComputationFrequency, name: nameof(BrainComputationFrequency));
			IsAlphabet = s.Serialize<bool>(IsAlphabet, name: nameof(IsAlphabet));
			s.SerializePadding(2, logIfNotNull: true);
		}
	}
}
