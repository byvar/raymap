namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterAimData : BinarySerializable {
		public MTH3D_Vector VertexPosition { get; set; }
		public int SuperObjectPersoId { get; set; }
		public MTH3D_Vector VertexShift { get; set; }
		public float ReachDistance { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VertexPosition = s.SerializeObject<MTH3D_Vector>(VertexPosition, name: nameof(VertexPosition));
			SuperObjectPersoId = s.Serialize<int>(SuperObjectPersoId, name: nameof(SuperObjectPersoId));
			VertexShift = s.SerializeObject<MTH3D_Vector>(VertexShift, name: nameof(VertexShift));
			ReachDistance = s.Serialize<float>(ReachDistance, name: nameof(ReachDistance));
		}
	}
}
