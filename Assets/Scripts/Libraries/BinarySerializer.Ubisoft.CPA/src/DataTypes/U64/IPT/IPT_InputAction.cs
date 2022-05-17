namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class IPT_InputAction : U64_Struct {
		public LST_List<IPT_InputElement> Elements { get; set; }
		public ushort KeyWordElementsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Elements = s.SerializeObject<LST_List<IPT_InputElement>>(Elements, name: nameof(Elements))?.Resolve(s);
			KeyWordElementsCount = s.Serialize<ushort>(KeyWordElementsCount, name: nameof(KeyWordElementsCount));

			// TODO: Parse
		}
	}
}
