namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_StandardGame : BinarySerializable {
		public const int AlwaysObjectType = 0x10000;
		public const int InvalidObjectType = -1;

		public int ObjectFamilyType { get; set; }
		public int ObjectModelType { get; set; }
		public int ObjectPersonalType { get; set; } // if > AlwaysObjectType: it's an always object

		public Pointer<HIE_SuperObject> SuperObject { get; set; }

		public GAM_ObjectInitFlags InitFlagWhenOutOfZone { get; set; }
		public GAM_ObjectInitFlags InitFlagWhenDeadOrTaken { get; set; }

		public uint Unknown_StdGameUInt { get; set; }

		public uint LastModificationFrame { get; set; } // "LastTrame"

		public GAM_ActorCapabilities Capabilities { get; set; } // Capabilities of object to follow a path in a graph

		public byte TractionFactor { get; set; }
		public byte HitPoints { get; set; }
		public byte HitPointsMax { get; set; }
		public byte HitPointsMaxMax { get; set; }

		public GAM_CustomBits CustomBits { get; set; }
		public GAM_AICustomBits AICustomBits { get; set; }

		public GAM_PlatformType PlatformType { get; set; }
		public GAM_MiscFlags MiscFlags { get; set; }
		public byte TransparencyZoneMin { get; set; }
		public byte TransparencyZoneMax { get; set; }

		public GAM_CustomBits SaveCustomBits { get; set; }
		public GAM_AICustomBits SaveAICustomBits { get; set; }
		public byte SaveHitPoints { get; set; }
		public byte SaveHitPointsMax { get; set; }
		public GAM_MiscFlags SaveMiscFlags { get; set; }

		public byte TooFarLimit { get; set; }

		// Editor only
		public bool IsLoadedInFix { get; set; }
		public bool IsInAllSubMaps { get; set; }

		public byte Importance { get; set; }
		public byte Optional { get; set; }

		public const int CustomCount = 2;
		public float[] CustomFloats { get; set; }
		public int[] CustomLongs { get; set; }
		public MTH3D_Vector[] CustomVectors { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			// TODO: Implement differences for Revolution, Largo Winch, Hype PS2

			ObjectFamilyType = s.Serialize<int>(ObjectFamilyType, name: nameof(ObjectFamilyType));
			ObjectModelType = s.Serialize<int>(ObjectModelType, name: nameof(ObjectModelType));
			ObjectPersonalType = s.Serialize<int>(ObjectPersonalType, name: nameof(ObjectPersonalType));
			
			SuperObject = s.SerializePointer<HIE_SuperObject>(SuperObject, name: nameof(SuperObject))?.ResolveObject(s);

			InitFlagWhenOutOfZone = s.Serialize<GAM_ObjectInitFlags>(InitFlagWhenOutOfZone, name: nameof(InitFlagWhenOutOfZone));
			InitFlagWhenDeadOrTaken = s.Serialize<GAM_ObjectInitFlags>(InitFlagWhenDeadOrTaken, name: nameof(InitFlagWhenDeadOrTaken));
			s.Align(4, Offset);

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3) && s.GetCPASettings().Platform != Platform.DC) {
				Unknown_StdGameUInt = s.Serialize<uint>(Unknown_StdGameUInt, name: nameof(Unknown_StdGameUInt));
			}

			LastModificationFrame = s.Serialize<uint>(LastModificationFrame, name: nameof(LastModificationFrame));

			Capabilities = s.Serialize<GAM_ActorCapabilities>(Capabilities, name: nameof(Capabilities));

			TractionFactor = s.Serialize<byte>(TractionFactor, name: nameof(TractionFactor));
			HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
			HitPointsMax = s.Serialize<byte>(HitPointsMax, name: nameof(HitPointsMax));
			HitPointsMaxMax = s.Serialize<byte>(HitPointsMaxMax, name: nameof(HitPointsMaxMax));

			CustomBits = s.Serialize<GAM_CustomBits>(CustomBits, name: nameof(CustomBits));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				AICustomBits = s.Serialize<GAM_AICustomBits>(AICustomBits, name: nameof(AICustomBits));
			}

			PlatformType = s.Serialize<GAM_PlatformType>(PlatformType, name: nameof(PlatformType));
			MiscFlags = s.Serialize<GAM_MiscFlags>(MiscFlags, name: nameof(MiscFlags));
			TransparencyZoneMin = s.Serialize<byte>(TransparencyZoneMin, name: nameof(TransparencyZoneMin));
			TransparencyZoneMax = s.Serialize<byte>(TransparencyZoneMax, name: nameof(TransparencyZoneMax));

			SaveCustomBits = s.Serialize<GAM_CustomBits>(SaveCustomBits, name: nameof(SaveCustomBits));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				SaveAICustomBits = s.Serialize<GAM_AICustomBits>(SaveAICustomBits, name: nameof(SaveAICustomBits));
			}
			SaveHitPoints = s.Serialize<byte>(SaveHitPoints, name: nameof(SaveHitPoints));
			SaveHitPointsMax = s.Serialize<byte>(SaveHitPointsMax, name: nameof(SaveHitPointsMax));
			SaveMiscFlags = s.Serialize<GAM_MiscFlags>(SaveMiscFlags, name: nameof(SaveMiscFlags));

			TooFarLimit = s.Serialize<byte>(TooFarLimit, name: nameof(TooFarLimit));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Importance = s.Serialize<byte>(Importance, name: nameof(Importance));
				Optional = s.Serialize<byte>(Optional, name: nameof(Optional));
				s.Align(4, Offset);
				CustomFloats = s.SerializeArray<float>(CustomFloats, CustomCount, name: nameof(CustomFloats));
				CustomLongs = s.SerializeArray<int>(CustomLongs, CustomCount, name: nameof(CustomLongs));
				CustomVectors = s.SerializeObjectArray<MTH3D_Vector>(CustomVectors, CustomCount, name: nameof(CustomVectors));
			}
		}
	}
}
