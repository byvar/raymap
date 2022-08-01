namespace BinarySerializer.Ubisoft.CPA {
	public class LS_LipsSynchValue : BinarySerializable {
		public byte Code { get; set; }
		public MTH3D_Vector Position { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Code = s.Serialize<byte>(Code, name: nameof(Code));
			s.Align(4, Offset);
			Position = s.SerializeObject<MTH3D_Vector>(Position, name: nameof(Position));
		}
	}
}
