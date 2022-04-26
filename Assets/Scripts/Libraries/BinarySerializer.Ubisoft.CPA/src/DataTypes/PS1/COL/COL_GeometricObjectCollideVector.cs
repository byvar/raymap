namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class COL_GeometricObjectCollideVector : BinarySerializable
	{
		public float? Pre_CoordinateScale { get; set; }

		public MTH3D_Vector_PS1_Short Vector { get; set; }
		public short GarbageData { get; set; } // ???

		public override void SerializeImpl(SerializerObject s)
		{
			Vector = s.SerializeObject<MTH3D_Vector_PS1_Short>(Vector, onPreSerialize: v => v.Pre_CoordinateScale = Pre_CoordinateScale, name: nameof(Vector));
			GarbageData = s.Serialize<short>(GarbageData, name: nameof(GarbageData));
		}
	}
}