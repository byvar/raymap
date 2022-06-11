namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_GenericMemory : U64_Struct {
		public U64_GenericReference Memory { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Memory = s.SerializeObject<U64_GenericReference>(Memory, name: nameof(Memory))?.Resolve(s);
		}
	}
}
