namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Rectangle : SNA_Description_Data {
		public int XMin { get; set; }
		public int YMin { get; set; }
		public int XMax { get; set; }
		public int YMax { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			XMin = s.Serialize<int>(XMin, name: nameof(XMin));
			YMin = s.Serialize<int>(YMin, name: nameof(YMin));
			XMax = s.Serialize<int>(XMax, name: nameof(XMax));
			YMax = s.Serialize<int>(YMax, name: nameof(YMax));
		}
	}
}
