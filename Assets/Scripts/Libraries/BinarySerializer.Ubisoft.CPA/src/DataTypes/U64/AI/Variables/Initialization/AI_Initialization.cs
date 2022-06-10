namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Initialization : U64_Struct {
		public LST_ReferenceList<AI_InitializationVariable> Variables { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Variables = s.SerializeObject<LST_ReferenceList<AI_InitializationVariable>>(Variables, name: nameof(Variables))?.Resolve(s);
		}
	}
}
