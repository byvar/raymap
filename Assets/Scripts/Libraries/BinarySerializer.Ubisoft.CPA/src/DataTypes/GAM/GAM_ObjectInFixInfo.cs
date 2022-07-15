namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ObjectInFixInfo : BinarySerializable {
		public Pointer ObjectOffset { get; set; }

		public bool ShouldModifyData { get; set; }
		public Pointer SuperObjectOffset { get; set; }
		public byte[] Matrix3DData { get; set; }
		public uint SuperObjectFlags { get; set; }
		public float TransparencyLevel { get; set; }
		public Pointer LST2_Next { get; set; }
		public Pointer LST2_Previous { get; set; }
		public Pointer LST2_Father { get; set; }
		public override void SerializeImpl(SerializerObject s) {
			ObjectOffset = s.SerializePointer(ObjectOffset, name: nameof(ObjectOffset));

			ShouldModifyData = HasStandardGameSuperObject(s);
			if (ShouldModifyData) {
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					SuperObjectOffset = s.SerializePointer(SuperObjectOffset, name: nameof(SuperObjectOffset));
					Matrix3DData = s.SerializeArray<byte>(Matrix3DData, 0x58, name: nameof(Matrix3DData));
					SuperObjectFlags = s.Serialize<uint>(SuperObjectFlags, name: nameof(SuperObjectFlags));
					TransparencyLevel = s.Serialize<float>(TransparencyLevel, name: nameof(TransparencyLevel));
					LST2_Next = s.SerializePointer(LST2_Next, name: nameof(LST2_Next));
					LST2_Previous = s.SerializePointer(LST2_Previous, name: nameof(LST2_Previous));
					LST2_Father = s.SerializePointer(LST2_Father, name: nameof(LST2_Father));
				} else {
					Matrix3DData = s.SerializeArray<byte>(Matrix3DData, 0x58, name: nameof(Matrix3DData));
					SuperObjectOffset = s.SerializePointer(SuperObjectOffset, name: nameof(SuperObjectOffset));
				}
			}
		}

		public void Apply(SerializerObject s) {
			if(!ShouldModifyData) return;
			s.DoAt(SuperObjectOffset, () => {
				// TODO
			});
		}

		private bool HasStandardGameSuperObject(SerializerObject s) {
			bool hasSO = false;
			s.DoAt(ObjectOffset, () => {
				var unknown = s.Serialize<uint>(default, name: "PointerCheck_3dData");
				Pointer stdGame = s.SerializePointer(default, allowInvalid: true, name: "PointerCheck_StdGame");
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					s.DoAt(stdGame, () => {
						s.SerializeArray<uint>(default, 3, name: "PointerCheck_ObjectTypes");
						Pointer so = s.SerializePointer(default, allowInvalid: true, name: "PointerCheck_StdGameSuperObject");
						if (so != null) hasSO = true;
					});
				} else {
					// Tonic Trouble doesn't require further checks
					if(stdGame != null) hasSO = true;
				}
			});
			return hasSO;
		}
	}
}
