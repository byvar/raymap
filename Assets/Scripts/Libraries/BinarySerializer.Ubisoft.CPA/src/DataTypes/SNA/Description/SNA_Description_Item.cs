using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Item : BinarySerializable {
		public SNA_Description_Data Pre_ParentData { get; set; }

		public int TypeInt { get; set; }
		public SNA_DescriptionType Type {
			get {
				var snaTypes = Context.GetCPASettings().SNATypes;
				return snaTypes.GetType(TypeInt);
			}
			set {
				var snaTypes = Context.GetCPASettings().SNATypes;
				TypeInt = snaTypes.GetInt(value);
			}
		}
		public SNA_Description_Data Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TypeInt = s.Serialize<int>(TypeInt, name: nameof(TypeInt));
			s.Log("Type: {0}", Type);

			var snaTypes = Context.GetCPASettings().SNATypes;
			var type = snaTypes.GetType(TypeInt);

			void SerializeData<T>() where T : SNA_Description_Data, new() {
				Data = s.SerializeObject<T>((T)Data, name: nameof(Data));
			}

			switch (type) {
				case SNA_DescriptionType.MemoryDescTitle:
				case SNA_DescriptionType.DirectoryDescTitle:
				case SNA_DescriptionType.BigFileDescTitle:
				case SNA_DescriptionType.VignetteDescTitle:
				case SNA_DescriptionType.LevelDscTitle:
				case SNA_DescriptionType.LevelDscLevelSoundBanks:
				case SNA_DescriptionType.GameOptionDescTitle:
				case SNA_DescriptionType.ActivateDeviceTitle:
					SerializeData<SNA_Description_Section>();
					break;

				case SNA_DescriptionType.RandomDescTitle:
					SerializeData<SNA_Description_RandomDesc>();
					break;

				case SNA_DescriptionType.LevelNameTitle:
					SerializeData<SNA_Description_LevelNameSection>();
					break;

				case SNA_DescriptionType.GAMFixMemory:
				case SNA_DescriptionType.ACPFixMemory:
				case SNA_DescriptionType.ACPTextMemory:
				case SNA_DescriptionType.AIFixMemory:
				case SNA_DescriptionType.TMPFixMemory:
				case SNA_DescriptionType.IPTMemory:
				case SNA_DescriptionType.SAIFixMemory:
				case SNA_DescriptionType.FontMemory:
				case SNA_DescriptionType.PositionMemory:
				case SNA_DescriptionType.GAMLevelMemory:
				case SNA_DescriptionType.AILevelMemory:
				case SNA_DescriptionType.ACPLevelMemory:
				case SNA_DescriptionType.SAILevelMemory:
				case SNA_DescriptionType.TMPLevelMemory:
				case SNA_DescriptionType.TT_LipsSynchMemory:
				case SNA_DescriptionType.TT_PLAMaxSuperObject:
				case SNA_DescriptionType.TT_PLAMaxMatrix:

				case SNA_DescriptionType.MaxValueBar:

				case SNA_DescriptionType.InitInputDeviceManager: // HistoricSize

				case SNA_DescriptionType.ActivatePadAction: // PadNumber
				case SNA_DescriptionType.ActivateJoystickAction: // PadNumber

				case SNA_DescriptionType.LevelLoadSoundBank: // Bank index
					SerializeData<SNA_Description_Long>();
					break;

				case SNA_DescriptionType.ScriptMemory:
				case SNA_DescriptionType.TT_MenuMemory:
				case SNA_DescriptionType.TT_InventoryMemory:
				case SNA_DescriptionType.TT_FontMemory:
					SerializeData<SNA_Description_LongLong>();
					break;

				case SNA_DescriptionType.DirectoryOfEngineDLL:
				case SNA_DescriptionType.DirectoryOfGameData:
				case SNA_DescriptionType.DirectoryOfTexts:
				case SNA_DescriptionType.DirectoryOfWorld:
				case SNA_DescriptionType.DirectoryOfLevels:
				case SNA_DescriptionType.DirectoryOfFamilies:
				case SNA_DescriptionType.DirectoryOfCharacters:
				case SNA_DescriptionType.DirectoryOfAnimations:
				case SNA_DescriptionType.DirectoryOfGraphicsClasses:
				case SNA_DescriptionType.DirectoryOfGraphicsBanks:
				case SNA_DescriptionType.DirectoryOfMechanics:
				case SNA_DescriptionType.DirectoryOfSound:
				case SNA_DescriptionType.DirectoryOfVisuals:
				case SNA_DescriptionType.DirectoryOfEnvironment:
				case SNA_DescriptionType.DirectoryOfMaterials:
				case SNA_DescriptionType.DirectoryOfSaveGame:
				case SNA_DescriptionType.DirectoryOfExtras:
				case SNA_DescriptionType.DirectoryOfTexture:
				case SNA_DescriptionType.DirectoryOfVignettes:
				case SNA_DescriptionType.DirectoryOfOptions:
				case SNA_DescriptionType.DirectoryOfLipsSync:
				case SNA_DescriptionType.DirectoryOfZdx:
				case SNA_DescriptionType.DirectoryOfEffects:

				case SNA_DescriptionType.BigFileVignettes:
				case SNA_DescriptionType.BigFileTextures:
				case SNA_DescriptionType.TT_BigFileCredits:

				case SNA_DescriptionType.LoadVignette:
				case SNA_DescriptionType.LoadLevelVignette:

				case SNA_DescriptionType.DefaultFile:
				case SNA_DescriptionType.CurrentFile:

				case SNA_DescriptionType.LevelName:
				case SNA_DescriptionType.TT_CreditsLevelName:
					SerializeData<SNA_Description_String>();
					break;

				case SNA_DescriptionType.InitBarOutlineColor:
				case SNA_DescriptionType.InitBarInsideColor:
					SerializeData<SNA_Description_Color>();
					break;

				case SNA_DescriptionType.InitBarColor:
					SerializeData<SNA_Description_Gradient>();
					break;

				case SNA_DescriptionType.CreateBar:
					SerializeData<SNA_Description_Rectangle>();
					break;

				case SNA_DescriptionType.RandomReadTable:
					Data = s.SerializeObject<SNA_Description_RandomTable>(
						(SNA_Description_RandomTable)Data,
						onPreSerialize: t => t.Pre_RandomDesc = (SNA_Description_RandomDesc)Pre_ParentData,
						name: nameof(Data));
					break;

				case SNA_DescriptionType.FrameSynchro:
					SerializeData<SNA_Description_FrameSynchro>();
					break;

				case SNA_DescriptionType.LevelLoadMap:
					SerializeData<SNA_Description_SoundBanksArray>();
					break;

				case SNA_DescriptionType.EndOfDescSection:

				case SNA_DescriptionType.InitVignette:
				case SNA_DescriptionType.FreeVignette:
				case SNA_DescriptionType.DisplayVignette:
				case SNA_DescriptionType.AddBar:

				case SNA_DescriptionType.ActivateKeyboardAction:
				case SNA_DescriptionType.ActivateMouseAction:

				case SNA_DescriptionType.RandomComputeTable:
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
