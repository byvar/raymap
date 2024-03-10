using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using System;

namespace Raymap {
	public abstract class CPA_BaseManager : BaseGameManager {
		public override void AddContextSettings(Context context) {
			base.AddContextSettings(context);
			var settings = context.GetMapViewerSettings();
			var cpaMode = GetCPAMode(settings);
			var cpaSettings = CPA_Settings_Defines.GetSettings(cpaMode);
			context.AddSettings<CPA_Settings>(cpaSettings);
		}

		protected CPA_GameMode GetCPAMode(MapViewerSettings settings) {
			return settings.GameModeSelection switch {

				GameModeSelection.Rayman2PC => CPA_GameMode.Rayman2PC,
				GameModeSelection.Rayman2PCDemo_1999_08_18 => CPA_GameMode.Rayman2PCDemo_1999_08_18,
				GameModeSelection.Rayman2PCDemo_1999_09_04 => CPA_GameMode.Rayman2PCDemo_1999_09_04,
				GameModeSelection.Rayman2DC => CPA_GameMode.Rayman2DC,
				GameModeSelection.Rayman2DCDevBuild_1999_11_22 => CPA_GameMode.Rayman2DCDevBuild_1999_11_22,
				GameModeSelection.Rayman2IOS => CPA_GameMode.Rayman2IOS,
				GameModeSelection.Rayman2IOSDemo => CPA_GameMode.Rayman2IOSDemo,
				GameModeSelection.Rayman2PS1 => CPA_GameMode.Rayman2PS1,
				GameModeSelection.Rayman2PS1Demo => CPA_GameMode.Rayman2PS1Demo,
				GameModeSelection.Rayman2PS1Demo_SLUS_90095 => CPA_GameMode.Rayman2PS1Demo_SLUS_90095,
				GameModeSelection.Rayman2PS2 => CPA_GameMode.Rayman2PS2,
				GameModeSelection.Rayman2N64 => CPA_GameMode.Rayman2N64,
				GameModeSelection.Rayman2DS => CPA_GameMode.Rayman2DS,
				GameModeSelection.Rayman23DS => CPA_GameMode.Rayman23DS,

				GameModeSelection.RaymanMPC => CPA_GameMode.RaymanMPC,
				GameModeSelection.RaymanMPS2 => CPA_GameMode.RaymanMPS2,
				GameModeSelection.RaymanMPS2Demo_2001_07_25 => CPA_GameMode.RaymanMPS2Demo_2001_07_25,
				GameModeSelection.RaymanArenaPC => CPA_GameMode.RaymanArenaPC,
				GameModeSelection.RaymanArenaPS2 => CPA_GameMode.RaymanArenaPS2,
				GameModeSelection.RaymanArenaGC => CPA_GameMode.RaymanArenaGC,
				GameModeSelection.RaymanArenaGCDemo_2002_03_07 => CPA_GameMode.RaymanArenaGCDemo_2002_03_07,
				GameModeSelection.RaymanArenaXbox => CPA_GameMode.RaymanArenaXbox,
				GameModeSelection.RaymanRushPS1 => CPA_GameMode.RaymanRushPS1,

				GameModeSelection.Rayman3PC => CPA_GameMode.Rayman3PC,
				GameModeSelection.Rayman3PCDemo_2002_10_01 => CPA_GameMode.Rayman3PCDemo_2002_10_01,
				GameModeSelection.Rayman3PCDemo_2002_10_21 => CPA_GameMode.Rayman3PCDemo_2002_10_21,
				GameModeSelection.Rayman3PCDemo_2002_12_09 => CPA_GameMode.Rayman3PCDemo_2002_12_09,
				GameModeSelection.Rayman3PCDemo_2003_01_06 => CPA_GameMode.Rayman3PCDemo_2003_01_06,
				GameModeSelection.Rayman3MacOS => CPA_GameMode.Rayman3MacOS,
				GameModeSelection.Rayman3GC => CPA_GameMode.Rayman3GC,
				GameModeSelection.Rayman3PS2 => CPA_GameMode.Rayman3PS2,
				GameModeSelection.Rayman3PS2Demo_2002_05_17 => CPA_GameMode.Rayman3PS2Demo_2002_05_17,
				GameModeSelection.Rayman3PS2Demo_2002_08_07 => CPA_GameMode.Rayman3PS2Demo_2002_08_07,
				GameModeSelection.Rayman3PS2DevBuild_2002_09_06 => CPA_GameMode.Rayman3PS2DevBuild_2002_09_06,
				GameModeSelection.Rayman3PS2Demo_2002_10_29 => CPA_GameMode.Rayman3PS2Demo_2002_10_29,
				GameModeSelection.Rayman3PS2Demo_2002_12_18 => CPA_GameMode.Rayman3PS2Demo_2002_12_18,
				GameModeSelection.Rayman3Xbox => CPA_GameMode.Rayman3Xbox,
				GameModeSelection.Rayman3Xbox360 => CPA_GameMode.Rayman3Xbox360,
				GameModeSelection.Rayman3PS3 => CPA_GameMode.Rayman3PS3,

				GameModeSelection.RaymanRavingRabbidsDS => CPA_GameMode.RaymanRavingRabbidsDS,
				GameModeSelection.RaymanRavingRabbidsDSDevBuild_2006_05_25 => CPA_GameMode.RaymanRavingRabbidsDSDevBuild_2006_05_25,

				GameModeSelection.TonicTroublePC => CPA_GameMode.TonicTroublePC,
				GameModeSelection.TonicTroubleSEPC => CPA_GameMode.TonicTroubleSEPC,
				GameModeSelection.TonicTroubleN64 => CPA_GameMode.TonicTroubleN64,

				GameModeSelection.DonaldDuckPC => CPA_GameMode.DonaldDuckPC,
				GameModeSelection.DonaldDuckPCDemo => CPA_GameMode.DonaldDuckPCDemo,
				GameModeSelection.DonaldDuckDC => CPA_GameMode.DonaldDuckDC,
				GameModeSelection.DonaldDuckN64 => CPA_GameMode.DonaldDuckN64,
				GameModeSelection.DonaldDuckPS1 => CPA_GameMode.DonaldDuckPS1,
				GameModeSelection.DonaldDuckPKGC => CPA_GameMode.DonaldDuckPKGC,
				GameModeSelection.DinosaurPC => CPA_GameMode.DinosaurPC,
				GameModeSelection.LargoWinchPC => CPA_GameMode.LargoWinchPC,
				GameModeSelection.JungleBookPS1 => CPA_GameMode.JungleBookPS1,

				GameModeSelection.PlaymobilHypePC => CPA_GameMode.PlaymobilHypePC,
				GameModeSelection.PlaymobilLauraPC => CPA_GameMode.PlaymobilLauraPC,
				GameModeSelection.PlaymobilLauraPCPentiumIII => CPA_GameMode.PlaymobilLauraPCPentiumIII,
				GameModeSelection.PlaymobilAlexPC => CPA_GameMode.PlaymobilAlexPC,

				GameModeSelection.VIPPS1 => CPA_GameMode.VIPPS1,
				GameModeSelection.RedPlanetPC => CPA_GameMode.RedPlanetPC,

				_ => throw new Exception($"GameModeSelection value {settings.GameModeSelection} is not a valid CPA mode")
			};
		}
	}
}
