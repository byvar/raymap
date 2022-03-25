namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CAM_CameraModifier : BinarySerializable
	{
		public uint Type { get; set; }
		public int Int_04 { get; set; }
		public int Int_08 { get; set; }
		public int Int_0C { get; set; }
		public int Int_10 { get; set; }
		public int Int_14 { get; set; }
		public int Int_18 { get; set; }
		public int Int_1C { get; set; }
		public Pointer DataPointer { get; set; }
		public int X2 { get; set; }
		public int Y2 { get; set; }
		public int Z2 { get; set; }
		public int Int_30 { get; set; }
		public int Int_34 { get; set; }
		public int X_7 { get; set; }
		public int Y_7 { get; set; }
		public int Z_7 { get; set; }
		public int Int_44 { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public int Int_54 { get; set; }
		public int Int_58 { get; set; }
		public int Int_5C { get; set; }
		public int Int_60 { get; set; }
		public int Int_64 { get; set; }

		// Serialized from pointers
		public CAM_CameraGraph Graph { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
			Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
			Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
			Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
			Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
			Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
			Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));
			DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
			X2 = s.Serialize<int>(X2, name: nameof(X2));
			Y2 = s.Serialize<int>(Y2, name: nameof(Y2));
			Z2 = s.Serialize<int>(Z2, name: nameof(Z2));
			Int_30 = s.Serialize<int>(Int_30, name: nameof(Int_30));
			Int_34 = s.Serialize<int>(Int_34, name: nameof(Int_34));
			X_7 = s.Serialize<int>(X_7, name: nameof(X_7));
			Y_7 = s.Serialize<int>(Y_7, name: nameof(Y_7));
			Z_7 = s.Serialize<int>(Z_7, name: nameof(Z_7));
			Int_44 = s.Serialize<int>(Int_44, name: nameof(Int_44));
			X = s.Serialize<int>(X, name: nameof(X));
			Y = s.Serialize<int>(Y, name: nameof(Y));
			Z = s.Serialize<int>(Z, name: nameof(Z));
			Int_54 = s.Serialize<int>(Int_54, name: nameof(Int_54));
			Int_58 = s.Serialize<int>(Int_58, name: nameof(Int_58));
			Int_5C = s.Serialize<int>(Int_5C, name: nameof(Int_5C));
			Int_60 = s.Serialize<int>(Int_60, name: nameof(Int_60));
			Int_64 = s.Serialize<int>(Int_64, name: nameof(Int_64));

			// Serialize data from pointers
			if (Type == 9)
				s.DoAt(DataPointer, () => Graph = s.SerializeObject<CAM_CameraGraph>(Graph, name: nameof(Graph)));
			// TODO: SO if not type 9?
		}
	}
}