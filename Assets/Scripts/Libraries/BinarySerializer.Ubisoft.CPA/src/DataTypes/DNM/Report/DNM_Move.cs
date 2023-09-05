namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_Move : BinarySerializable {
		public MTH3D_Vector Linear { get; set; }
		public DNM_Rotation Angular { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Linear = s.SerializeObject<MTH3D_Vector>(Linear, name: nameof(Linear));
			Angular = s.SerializeObject<DNM_Rotation>(Angular, name: nameof(Angular));
		}
	}
}
