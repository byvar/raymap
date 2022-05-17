namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_BoundingVolumeBox : U64_Struct {
		public U64_ArrayReference<MTH3D_ShortVector> Values { get; set; } // Min and Max

		public override void SerializeImpl(SerializerObject s) {
			Values = s.SerializeObject<U64_ArrayReference<MTH3D_ShortVector>>(Values, name: nameof(Values))?.Resolve(s, 2);
		}
	}
}
