namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ObjectInFixInfo : BinarySerializable {
		public Pointer ObjectOffset { get; set; }

		public bool ShouldModifyData { get; set; }
		public Pointer SuperObjectOffset { get; set; }
		public byte[] Matrix3DData { get; set; }
		public uint SuperObjectFlags { get; set; }
		public uint TransparencyLevel { get; set; } // Actually a float
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
					TransparencyLevel = s.Serialize<uint>(TransparencyLevel, name: nameof(TransparencyLevel));
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
			SNA_DataBlockFile f = null;
			s.DoAt(SuperObjectOffset, () => {
				var soType = s.Serialize<uint>(default, name: "Check_SOType");
				var isPerso = s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2) ? soType == 2 : soType == 4;
				if (isPerso) {
					var engineObject = s.SerializePointer(default, name: "Check_EngineObject");
					s.DoAt(engineObject, () => {
						var _3dData = s.SerializePointer(default, name: "Check_3dData");
						s.DoAt(_3dData, () => {
							f = (SNA_DataBlockFile)s.CurrentBinaryFile;
							f.OverrideData(
								_3dData.FileOffset + (s.GetCPASettings().IsPressDemo ? 0x1C : 0x18),
								Matrix3DData);
						});
						// Tonic Trouble only
						if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
							var stdGame = s.SerializePointer(default, name: "Check_StdGame");
							s.DoAt(stdGame, () => {
								f = (SNA_DataBlockFile)s.CurrentBinaryFile;
								f.AddOverridePointer(stdGame.AbsoluteOffset + 0xC, SuperObjectOffset);
							});
						}
					});

					if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
						f = (SNA_DataBlockFile)SuperObjectOffset.File;
						f.AddOverridePointer(SuperObjectOffset.AbsoluteOffset + 0x14, LST2_Next);
						f.AddOverridePointer(SuperObjectOffset.AbsoluteOffset + 0x18, LST2_Previous);
						f.AddOverridePointer(SuperObjectOffset.AbsoluteOffset + 0x1C, LST2_Father);
						f.OverrideUInt(SuperObjectOffset.FileOffset + 0x30, SuperObjectFlags);
						f.OverrideUInt(SuperObjectOffset.FileOffset + 0x38, TransparencyLevel);
					}
				}
			});
		}

		private bool HasStandardGameSuperObject(SerializerObject s) {
			bool hasSO = false;
			s.DoAt(ObjectOffset, () => {
				var _3dData = s.Serialize<uint>(default, name: "PointerCheck_3dData");
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
