namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PS1_Matrix : BinarySerializable
	{
		// TODO: Serialize as array [3][3]
		public FixedPointInt16 Rot_M00 { get; set; }
		public FixedPointInt16 Rot_M01 { get; set; }
		public FixedPointInt16 Rot_M02 { get; set; }
		public FixedPointInt16 Rot_M10 { get; set; }
		public FixedPointInt16 Rot_M11 { get; set; }
		public FixedPointInt16 Rot_M12 { get; set; }
		public FixedPointInt16 Rot_M20 { get; set; }
		public FixedPointInt16 Rot_M21 { get; set; }
		public FixedPointInt16 Rot_M22 { get; set; }
		public ushort Uhsort_12 { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public ushort Ushort_20 { get; set; }
		public ushort Ushort_22 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Rot_M00 = s.SerializeObject<FixedPointInt16>(Rot_M00, x => x.Pre_PointPosition = 12, name: nameof(Rot_M00));
			Rot_M01 = s.SerializeObject<FixedPointInt16>(Rot_M01, x => x.Pre_PointPosition = 12, name: nameof(Rot_M01));
			Rot_M02 = s.SerializeObject<FixedPointInt16>(Rot_M02, x => x.Pre_PointPosition = 12, name: nameof(Rot_M02));
			Rot_M10 = s.SerializeObject<FixedPointInt16>(Rot_M10, x => x.Pre_PointPosition = 12, name: nameof(Rot_M10));
			Rot_M11 = s.SerializeObject<FixedPointInt16>(Rot_M11, x => x.Pre_PointPosition = 12, name: nameof(Rot_M11));
			Rot_M12 = s.SerializeObject<FixedPointInt16>(Rot_M12, x => x.Pre_PointPosition = 12, name: nameof(Rot_M12));
			Rot_M20 = s.SerializeObject<FixedPointInt16>(Rot_M20, x => x.Pre_PointPosition = 12, name: nameof(Rot_M20));
			Rot_M21 = s.SerializeObject<FixedPointInt16>(Rot_M21, x => x.Pre_PointPosition = 12, name: nameof(Rot_M21));
			Rot_M22 = s.SerializeObject<FixedPointInt16>(Rot_M22, x => x.Pre_PointPosition = 12, name: nameof(Rot_M22));
			Uhsort_12 = s.Serialize<ushort>(Uhsort_12, name: nameof(Uhsort_12));

			X = s.Serialize<int>(X, name: nameof(X));
			Y = s.Serialize<int>(Y, name: nameof(Y));
			Z = s.Serialize<int>(Z, name: nameof(Z));
			Ushort_20 = s.Serialize<ushort>(Ushort_20, name: nameof(Ushort_20));
			Ushort_22 = s.Serialize<ushort>(Ushort_22, name: nameof(Ushort_22));
		}
	}
}