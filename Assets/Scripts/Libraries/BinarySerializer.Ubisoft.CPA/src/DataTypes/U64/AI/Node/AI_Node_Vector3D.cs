namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Node_Vector3D : U64_Struct {
		public MTH3D_Vector Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeObject<MTH3D_Vector>(Value, name: nameof(Value));
		}
	}
}
