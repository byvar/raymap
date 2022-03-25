namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CAM_CameraModifierVolume : BinarySerializable
	{
		public uint Int_00 { get; set; }
		public uint Flags { get; set; }
		public int X0 { get; set; }
		public int Y0 { get; set; }
		public int Z0 { get; set; }
		public int Int_14 { get; set; }
		public int Radius { get; set; }
		public int X1 { get; set; }
		public int Y1 { get; set; }
		public int Z1 { get; set; }
		public int Int_28 { get; set; }
		public int Int_2C { get; set; }
		public int Int_30 { get; set; }
		public int Int_34 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Int_00 = s.Serialize<uint>(Int_00, name: nameof(Int_00));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			X0 = s.Serialize<int>(X0, name: nameof(X0));
			Y0 = s.Serialize<int>(Y0, name: nameof(Y0));
			Z0 = s.Serialize<int>(Z0, name: nameof(Z0));
			Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
			Radius = s.Serialize<int>(Radius, name: nameof(Radius));
			X1 = s.Serialize<int>(X1, name: nameof(X1));
			Y1 = s.Serialize<int>(Y1, name: nameof(Y1));
			Z1 = s.Serialize<int>(Z1, name: nameof(Z1));
			Int_28 = s.Serialize<int>(Int_28, name: nameof(Int_28));
			Int_2C = s.Serialize<int>(Int_2C, name: nameof(Int_2C));
			Int_30 = s.Serialize<int>(Int_30, name: nameof(Int_30));
			Int_34 = s.Serialize<int>(Int_34, name: nameof(Int_34));
		}
	}
}