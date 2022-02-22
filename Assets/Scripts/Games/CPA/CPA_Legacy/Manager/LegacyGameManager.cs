using BinarySerializer;
using Cysharp.Threading.Tasks;
using OpenSpace;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymap {
	public class LegacyGameManager : CPA_BaseManager {
		public override MapTreeNode GetLevels(MapViewerSettings settings) {
			var files = FindFiles(settings);
			var translation = GetSettings(settings)?.levelTranslation?.SortAndTranslate(files);

			MapTreeNode root = new MapTreeNode(null, null);

			if (translation != null) {
				root.Children = translation.Select(tr => new MapTreeNode(tr.Item1, tr.Item2)).ToArray();
			} else {
				root.Children = files.Select(filename => new MapTreeNode(filename, filename)).ToArray();
			}
			return root;
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			await UniTask.CompletedTask;
			throw new NotImplementedException();
		}

		protected virtual List<string> FindFiles(MapViewerSettings settings) {
			var directory = settings.GameDirectory;

			// Create the output
			var output = new List<string>();

			// If the directory does not exist, return the empty list
			if (!Directory.Exists(directory))
				return output;
			
			string[] filterPaths = new string[] {
				"fix",
				/*"ani0",
				"ani1",
				"ani2",
				"ani3",
				"ani4",
				"ani5",
				"ani6",
				"ani7",
				"ani8",
				"ani9",
				"ani10",*/
			};


			var cpaMode = GetCPAMode(settings);
			var cpaSettings = BinarySerializer.Ubisoft.CPA.CPA_Settings_Defines.GetSettings(cpaMode);

			// Add the found files containing the correct file extension
			string extension = null;
			string[] levels;
			switch (cpaSettings.Platform) {
				case BinarySerializer.Ubisoft.CPA.Platform.PC:
				case BinarySerializer.Ubisoft.CPA.Platform.iOS:
				case BinarySerializer.Ubisoft.CPA.Platform.GC:
				case BinarySerializer.Ubisoft.CPA.Platform.Xbox:
				case BinarySerializer.Ubisoft.CPA.Platform.Xbox360:
				case BinarySerializer.Ubisoft.CPA.Platform.PS3:
				case BinarySerializer.Ubisoft.CPA.Platform.MacOS:
					if (!cpaSettings.EngineVersionTree.HasParent(BinarySerializer.Ubisoft.CPA.EngineVersion.CPA_3)) {
						extension = "*.sna";
					} else {
						extension = "*.lvl";
					}
					break;
				case BinarySerializer.Ubisoft.CPA.Platform.DC: extension = "*.DAT"; break;
				case BinarySerializer.Ubisoft.CPA.Platform.PS2:
					if (!cpaSettings.EngineVersionTree.HasParent(BinarySerializer.Ubisoft.CPA.EngineVersion.CPA_3)) {
						if (!cpaSettings.EngineVersionTree.HasParent(BinarySerializer.Ubisoft.CPA.EngineVersion.CPA_Montreal)) {
							extension = "*.sna";
						} else {
							extension = "*.lv2";
						}
					} else {
						extension = "*.lvl";
					}
					break;
				case BinarySerializer.Ubisoft.CPA.Platform.PS1:
					Legacy_Settings.Init(GetSettings(settings));
					MapLoader.Reset();
					R2PS1Loader l1 = MapLoader.Loader as R2PS1Loader;
					l1.gameDataBinFolder = directory;
					levels = l1.LoadLevelList();
					MapLoader.Reset();
					output.AddRange(levels);
					Legacy_Settings.s = null;
					break;
				case BinarySerializer.Ubisoft.CPA.Platform.DS:
				case BinarySerializer.Ubisoft.CPA.Platform.N64:
				case BinarySerializer.Ubisoft.CPA.Platform._3DS:
					Legacy_Settings.Init(GetSettings(settings));
					MapLoader.Reset();
					R2ROMLoader lr = MapLoader.Loader as R2ROMLoader;
					lr.gameDataBinFolder = directory;
					levels = lr.LoadLevelList();
					MapLoader.Reset();
					output.AddRange(levels);
					Legacy_Settings.s = null;
					break;
			}
			if (extension != null) {
				output.AddRange(
					from file in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories)
					let filename = Path.GetFileNameWithoutExtension(file)
					let dirname = new DirectoryInfo(file).Parent.Name
					where ((!filterPaths.Contains(filename.ToLower()))
					&& dirname.ToLower() == filename.ToLower())
					select dirname

				);
			}
			//Debug.Log(string.Join("\n",output));

			// Return the output
			return output;
		}
		public void CreateSettings(MapViewerSettings settings) {
			Legacy_Settings.Init(GetSettings(settings));
		}

		public Legacy_Settings GetSettings(MapViewerSettings settings) {
			Legacy_Settings.Mode Mode = GetLegacyMode(settings);
			return Legacy_Settings.GetSettings(Mode);
		}

		public Legacy_Settings.Mode GetLegacyMode(MapViewerSettings settings) {
			var raymapMode = settings.GameModeSelection;
			Legacy_Settings.Mode legacyMode = raymapMode switch {
				GameModeSelection.Rayman2PC => Legacy_Settings.Mode.Rayman2PC,
				GameModeSelection.Rayman2PCDemo_1999_08_18 => Legacy_Settings.Mode.Rayman2PCDemo_1999_08_18,
				GameModeSelection.Rayman2PCDemo_1999_09_04 => Legacy_Settings.Mode.Rayman2PCDemo_1999_09_04,
				GameModeSelection.Rayman2DC => Legacy_Settings.Mode.Rayman2DC,
				GameModeSelection.Rayman2IOS => Legacy_Settings.Mode.Rayman2IOS,
				GameModeSelection.Rayman2IOSDemo => Legacy_Settings.Mode.Rayman2IOSDemo,
				GameModeSelection.Rayman2PS1 => Legacy_Settings.Mode.Rayman2PS1,
				GameModeSelection.Rayman2PS2 => Legacy_Settings.Mode.Rayman2PS2,
				GameModeSelection.Rayman2N64 => Legacy_Settings.Mode.Rayman2N64,
				GameModeSelection.Rayman2DS => Legacy_Settings.Mode.Rayman2DS,
				GameModeSelection.Rayman23DS => Legacy_Settings.Mode.Rayman23DS,
				GameModeSelection.RaymanMPC => Legacy_Settings.Mode.RaymanMPC,
				GameModeSelection.RaymanMPS2 => Legacy_Settings.Mode.RaymanMPS2,
				GameModeSelection.RaymanMPS2Demo_2001_07_25 => Legacy_Settings.Mode.RaymanMPS2Demo_2001_07_25,
				GameModeSelection.RaymanArenaPC => Legacy_Settings.Mode.RaymanArenaPC,
				GameModeSelection.RaymanArenaPS2 => Legacy_Settings.Mode.RaymanArenaPS2,
				GameModeSelection.RaymanArenaGC => Legacy_Settings.Mode.RaymanArenaGC,
				GameModeSelection.RaymanArenaGCDemo_2002_03_07 => Legacy_Settings.Mode.RaymanArenaGCDemo_2002_03_07,
				GameModeSelection.RaymanArenaXbox => Legacy_Settings.Mode.RaymanArenaXbox,
				GameModeSelection.RaymanRushPS1 => Legacy_Settings.Mode.RaymanRushPS1,
				GameModeSelection.Rayman3PC => Legacy_Settings.Mode.Rayman3PC,
				GameModeSelection.Rayman3PCDemo_2002_10_01 => Legacy_Settings.Mode.Rayman3PCDemo_2002_10_01,
				GameModeSelection.Rayman3PCDemo_2002_10_21 => Legacy_Settings.Mode.Rayman3PCDemo_2002_10_21,
				GameModeSelection.Rayman3PCDemo_2002_12_09 => Legacy_Settings.Mode.Rayman3PCDemo_2002_12_09,
				GameModeSelection.Rayman3PCDemo_2003_01_06 => Legacy_Settings.Mode.Rayman3PCDemo_2003_01_06,
				GameModeSelection.Rayman3MacOS => Legacy_Settings.Mode.Rayman3MacOS,
				GameModeSelection.Rayman3GC => Legacy_Settings.Mode.Rayman3GC,
				GameModeSelection.Rayman3PS2 => Legacy_Settings.Mode.Rayman3PS2,
				GameModeSelection.Rayman3PS2Demo_2002_05_17 => Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17,
				GameModeSelection.Rayman3PS2Demo_2002_08_07 => Legacy_Settings.Mode.Rayman3PS2Demo_2002_08_07,
				GameModeSelection.Rayman3PS2DevBuild_2002_09_06 => Legacy_Settings.Mode.Rayman3PS2DevBuild_2002_09_06,
				GameModeSelection.Rayman3PS2Demo_2002_10_29 => Legacy_Settings.Mode.Rayman3PS2Demo_2002_10_29,
				GameModeSelection.Rayman3PS2Demo_2002_12_18 => Legacy_Settings.Mode.Rayman3PS2Demo_2002_12_18,
				GameModeSelection.Rayman3Xbox => Legacy_Settings.Mode.Rayman3Xbox,
				GameModeSelection.Rayman3Xbox360 => Legacy_Settings.Mode.Rayman3Xbox360,
				GameModeSelection.Rayman3PS3 => Legacy_Settings.Mode.Rayman3PS3,
				GameModeSelection.RaymanRavingRabbidsDS => Legacy_Settings.Mode.RaymanRavingRabbidsDS,
				GameModeSelection.RaymanRavingRabbidsDSDevBuild_2006_05_25 => Legacy_Settings.Mode.RaymanRavingRabbidsDSDevBuild_2006_05_25,
				GameModeSelection.TonicTroublePC => Legacy_Settings.Mode.TonicTroublePC,
				GameModeSelection.TonicTroubleSEPC => Legacy_Settings.Mode.TonicTroubleSEPC,
				GameModeSelection.TonicTroubleN64 => Legacy_Settings.Mode.TonicTroubleN64,
				GameModeSelection.DonaldDuckPC => Legacy_Settings.Mode.DonaldDuckPC,
				GameModeSelection.DonaldDuckPCDemo => Legacy_Settings.Mode.DonaldDuckPCDemo,
				GameModeSelection.DonaldDuckDC => Legacy_Settings.Mode.DonaldDuckDC,
				GameModeSelection.DonaldDuckN64 => Legacy_Settings.Mode.DonaldDuckN64,
				GameModeSelection.DonaldDuckPS1 => Legacy_Settings.Mode.DonaldDuckPS1,
				GameModeSelection.DonaldDuckPKGC => Legacy_Settings.Mode.DonaldDuckPKGC,
				GameModeSelection.PlaymobilHypePC => Legacy_Settings.Mode.PlaymobilHypePC,
				GameModeSelection.PlaymobilLauraPC => Legacy_Settings.Mode.PlaymobilLauraPC,
				GameModeSelection.PlaymobilAlexPC => Legacy_Settings.Mode.PlaymobilAlexPC,
				GameModeSelection.DinosaurPC => Legacy_Settings.Mode.DinosaurPC,
				GameModeSelection.LargoWinchPC => Legacy_Settings.Mode.LargoWinchPC,
				GameModeSelection.JungleBookPS1 => Legacy_Settings.Mode.JungleBookPS1,
				GameModeSelection.VIPPS1 => Legacy_Settings.Mode.VIPPS1,
				GameModeSelection.RedPlanetPC => Legacy_Settings.Mode.RedPlanetPC,
				_ => throw new Exception($"Could not match Mode {raymapMode} with any CPA mode.")
			};
			return legacyMode;
		}
	}
}
