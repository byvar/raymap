namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class IPT_InputLink : U64_Struct {
		public LST_List<IPT_InputLinkElement> InputActions { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			InputActions = s.SerializeObject<LST_List<IPT_InputLinkElement>>(InputActions, name: nameof(InputActions))?.Resolve(s);
		}
	}
}
