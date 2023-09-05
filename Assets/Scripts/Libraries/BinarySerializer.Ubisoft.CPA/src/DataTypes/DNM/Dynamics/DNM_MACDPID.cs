namespace BinarySerializer.Ubisoft.CPA {
	// Module Allowing the Communication of Datas from the Player or the Intelligence to the Dynamics
	// ... o_o
	public class DNM_MACDPID : BinarySerializable {
		// Ok, I hate this struct. Some of the data here can mean 10 different things.
		public float Float_00 { get; set; }
		public MTH3D_Vector Vector_01 { get; set; }
		public MTH3D_Vector Vector_02 { get; set; }
		public MTH3D_Vector Vector_03 { get; set; }
		public float Float_03 { get; set; } // Yes, this is also a Data3.
		public float Float_04 { get; set; }
		public float Float_05 { get; set; }
		public DNM_Rotation Rotation_06 { get; set; }
		public DNM_Rotation Rotation_07 { get; set; }
		public sbyte SByte_08 { get; set; }
		public ushort UShort_09 { get; set; }
		public MTH3D_Vector Vector_10 { get; set; }
		public float Float_11 { get; set; }
		public MTH3D_Vector Vector_12 { get; set; }
		public float Float_13 { get; set; }
		public bool Bool_14 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Float_00 = s.Serialize<float>(Float_00, name: nameof(Float_00));
			Vector_01 = s.SerializeObject<MTH3D_Vector>(Vector_01, name: nameof(Vector_01));
			Vector_02 = s.SerializeObject<MTH3D_Vector>(Vector_02, name: nameof(Vector_02));
			Vector_03 = s.SerializeObject<MTH3D_Vector>(Vector_03, name: nameof(Vector_03));
			Float_03 = s.Serialize<float>(Float_03, name: nameof(Float_03));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_05 = s.Serialize<float>(Float_05, name: nameof(Float_05));
			Rotation_06 = s.SerializeObject<DNM_Rotation>(Rotation_06, name: nameof(Rotation_06));
			Rotation_07 = s.SerializeObject<DNM_Rotation>(Rotation_07, name: nameof(Rotation_07));
			SByte_08 = s.Serialize<sbyte>(SByte_08, name: nameof(SByte_08));
			s.Align(2, Offset);
			UShort_09 = s.Serialize<ushort>(UShort_09, name: nameof(UShort_09));
			Vector_10 = s.SerializeObject<MTH3D_Vector>(Vector_10, name: nameof(Vector_10));
			Float_11 = s.Serialize<float>(Float_11, name: nameof(Float_11));
			Vector_12 = s.SerializeObject<MTH3D_Vector>(Vector_12, name: nameof(Vector_12));
			Float_13 = s.Serialize<float>(Float_13, name: nameof(Float_13));
			Bool_14 = s.Serialize<bool>(Bool_14, name: nameof(Bool_14));
			s.Align(4, Offset);
		}
	}
}
