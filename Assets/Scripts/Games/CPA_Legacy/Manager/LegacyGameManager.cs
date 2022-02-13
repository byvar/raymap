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
	public class LegacyGameManager : BaseGameManager {
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

		public override UniTask<Unity_Level> LoadAsync(Context context) {
			throw new NotImplementedException();
		}

		private List<string> FindFiles(MapViewerSettings settings) {
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

			// Add the found files containing the correct file extension
			string extension = null;
			string[] levels;
			CPA_Settings.Init(GetSettings(settings));
			switch (CPA_Settings.s.platform) {
				case CPA_Settings.Platform.PC:
				case CPA_Settings.Platform.iOS:
				case CPA_Settings.Platform.GC:
				case CPA_Settings.Platform.Xbox:
				case CPA_Settings.Platform.Xbox360:
				case CPA_Settings.Platform.PS3:
				case CPA_Settings.Platform.MacOS:
					if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
						extension = "*.sna";
					} else {
						extension = "*.lvl";
					}
					break;
				case CPA_Settings.Platform.DC: extension = "*.DAT"; break;
				case CPA_Settings.Platform.PS2:
					if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
						if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R2) {
							extension = "*.sna";
						} else {
							extension = "*.lv2";
						}
					} else {
						extension = "*.lvl";
					}
					break;
				case CPA_Settings.Platform.PS1:
					MapLoader.Reset();
					R2PS1Loader l1 = MapLoader.Loader as R2PS1Loader;
					l1.gameDataBinFolder = directory;
					levels = l1.LoadLevelList();
					MapLoader.Reset();
					output.AddRange(levels);
					break;
				case CPA_Settings.Platform.DS:
				case CPA_Settings.Platform.N64:
				case CPA_Settings.Platform._3DS:
					MapLoader.Reset();
					R2ROMLoader lr = MapLoader.Loader as R2ROMLoader;
					lr.gameDataBinFolder = directory;
					levels = lr.LoadLevelList();
					MapLoader.Reset();
					output.AddRange(levels);
					break;
			}
			CPA_Settings.s = null;
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
			CPA_Settings.Init(GetSettings(settings));
		}

		public CPA_Settings GetSettings(MapViewerSettings settings) {
			CPA_Settings.Mode Mode = GetLegacyMode(settings);
			return CPA_Settings.GetSettings(Mode);
		}

		public CPA_Settings.Mode GetLegacyMode(MapViewerSettings settings) {
			var raymapMode = settings.GameModeSelection;
			CPA_Settings.Mode legacyMode = raymapMode switch {
				GameModeSelection.Rayman2PC => CPA_Settings.Mode.Rayman2PC,
				GameModeSelection.Rayman2PCDemo_1999_08_18 => CPA_Settings.Mode.Rayman2PCDemo_1999_08_18,
				GameModeSelection.Rayman2PCDemo_1999_09_04 => CPA_Settings.Mode.Rayman2PCDemo_1999_09_04,
				GameModeSelection.Rayman2DC => CPA_Settings.Mode.Rayman2DC,
				GameModeSelection.Rayman2IOS => CPA_Settings.Mode.Rayman2IOS,
				GameModeSelection.Rayman2IOSDemo => CPA_Settings.Mode.Rayman2IOSDemo,
				GameModeSelection.Rayman2PS1 => CPA_Settings.Mode.Rayman2PS1,
				GameModeSelection.Rayman2PS2 => CPA_Settings.Mode.Rayman2PS2,
				GameModeSelection.Rayman2N64 => CPA_Settings.Mode.Rayman2N64,
				GameModeSelection.Rayman2DS => CPA_Settings.Mode.Rayman2DS,
				GameModeSelection.Rayman23DS => CPA_Settings.Mode.Rayman23DS,
				GameModeSelection.RaymanMPC => CPA_Settings.Mode.RaymanMPC,
				GameModeSelection.RaymanMPS2 => CPA_Settings.Mode.RaymanMPS2,
				GameModeSelection.RaymanMPS2Demo_2001_07_25 => CPA_Settings.Mode.RaymanMPS2Demo_2001_07_25,
				GameModeSelection.RaymanArenaPC => CPA_Settings.Mode.RaymanArenaPC,
				GameModeSelection.RaymanArenaPS2 => CPA_Settings.Mode.RaymanArenaPS2,
				GameModeSelection.RaymanArenaGC => CPA_Settings.Mode.RaymanArenaGC,
				GameModeSelection.RaymanArenaGCDemo_2002_03_07 => CPA_Settings.Mode.RaymanArenaGCDemo_2002_03_07,
				GameModeSelection.RaymanArenaXbox => CPA_Settings.Mode.RaymanArenaXbox,
				GameModeSelection.RaymanRushPS1 => CPA_Settings.Mode.RaymanRushPS1,
				GameModeSelection.Rayman3PC => CPA_Settings.Mode.Rayman3PC,
				GameModeSelection.Rayman3PCDemo_2002_10_01 => CPA_Settings.Mode.Rayman3PCDemo_2002_10_01,
				GameModeSelection.Rayman3PCDemo_2002_10_21 => CPA_Settings.Mode.Rayman3PCDemo_2002_10_21,
				GameModeSelection.Rayman3PCDemo_2002_12_09 => CPA_Settings.Mode.Rayman3PCDemo_2002_12_09,
				GameModeSelection.Rayman3PCDemo_2003_01_06 => CPA_Settings.Mode.Rayman3PCDemo_2003_01_06,
				GameModeSelection.Rayman3MacOS => CPA_Settings.Mode.Rayman3MacOS,
				GameModeSelection.Rayman3GC => CPA_Settings.Mode.Rayman3GC,
				GameModeSelection.Rayman3PS2 => CPA_Settings.Mode.Rayman3PS2,
				GameModeSelection.Rayman3PS2Demo_2002_05_17 => CPA_Settings.Mode.Rayman3PS2Demo_2002_05_17,
				GameModeSelection.Rayman3PS2Demo_2002_08_07 => CPA_Settings.Mode.Rayman3PS2Demo_2002_08_07,
				GameModeSelection.Rayman3PS2DevBuild_2002_09_06 => CPA_Settings.Mode.Rayman3PS2DevBuild_2002_09_06,
				GameModeSelection.Rayman3PS2Demo_2002_10_29 => CPA_Settings.Mode.Rayman3PS2Demo_2002_10_29,
				GameModeSelection.Rayman3PS2Demo_2002_12_18 => CPA_Settings.Mode.Rayman3PS2Demo_2002_12_18,
				GameModeSelection.Rayman3Xbox => CPA_Settings.Mode.Rayman3Xbox,
				GameModeSelection.Rayman3Xbox360 => CPA_Settings.Mode.Rayman3Xbox360,
				GameModeSelection.Rayman3PS3 => CPA_Settings.Mode.Rayman3PS3,
				GameModeSelection.RaymanRavingRabbidsDS => CPA_Settings.Mode.RaymanRavingRabbidsDS,
				GameModeSelection.RaymanRavingRabbidsDSDevBuild_2006_05_25 => CPA_Settings.Mode.RaymanRavingRabbidsDSDevBuild_2006_05_25,
				GameModeSelection.TonicTroublePC => CPA_Settings.Mode.TonicTroublePC,
				GameModeSelection.TonicTroubleSEPC => CPA_Settings.Mode.TonicTroubleSEPC,
				GameModeSelection.TonicTroubleN64 => CPA_Settings.Mode.TonicTroubleN64,
				GameModeSelection.DonaldDuckPC => CPA_Settings.Mode.DonaldDuckPC,
				GameModeSelection.DonaldDuckPCDemo => CPA_Settings.Mode.DonaldDuckPCDemo,
				GameModeSelection.DonaldDuckDC => CPA_Settings.Mode.DonaldDuckDC,
				GameModeSelection.DonaldDuckN64 => CPA_Settings.Mode.DonaldDuckN64,
				GameModeSelection.DonaldDuckPS1 => CPA_Settings.Mode.DonaldDuckPS1,
				GameModeSelection.DonaldDuckPKGC => CPA_Settings.Mode.DonaldDuckPKGC,
				GameModeSelection.PlaymobilHypePC => CPA_Settings.Mode.PlaymobilHypePC,
				GameModeSelection.PlaymobilLauraPC => CPA_Settings.Mode.PlaymobilLauraPC,
				GameModeSelection.PlaymobilAlexPC => CPA_Settings.Mode.PlaymobilAlexPC,
				GameModeSelection.DinosaurPC => CPA_Settings.Mode.DinosaurPC,
				GameModeSelection.LargoWinchPC => CPA_Settings.Mode.LargoWinchPC,
				GameModeSelection.JungleBookPS1 => CPA_Settings.Mode.JungleBookPS1,
				GameModeSelection.VIPPS1 => CPA_Settings.Mode.VIPPS1,
				GameModeSelection.RedPlanetPC => CPA_Settings.Mode.RedPlanetPC,
				_ => throw new Exception($"Could not match Mode {raymapMode} with any CPA mode.")
			};
			return legacyMode;
		}
	}
}
