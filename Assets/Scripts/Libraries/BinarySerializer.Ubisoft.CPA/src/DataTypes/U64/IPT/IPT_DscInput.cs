namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class IPT_DscInput : U64_Struct {
		public int HistoricSize { get; set; }
		public LST_List<IPT_DscInputAction> InputActions { get; set; } // Not included in the ROM for some reason...

		public override void SerializeImpl(SerializerObject s) {
			HistoricSize = s.Serialize<int>(HistoricSize, name: nameof(HistoricSize));
			InputActions = s.SerializeObject<LST_List<IPT_DscInputAction>>(InputActions, name: nameof(InputActions)); //?.Resolve(s);
		}
	}
}
