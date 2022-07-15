using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class CPA_Globals_SNA : CPA_Globals {

		public CPA_Globals_SNA(Context c) : base(c) { }

		public SNA_File<SNA_Description> GameDSB { get; set; }
		public SNA_RelocationBigFile RelocationBigFile { get; set; }

		public override string GameDataDirectory =>
			Context.GetCPASettings().ApplyPathCapitalization(DirectoryDescription?.GetDirectory(SNA_DescriptionType.DirectoryOfGameData) ?? "GameData", PathCapitalizationType.All);

		public string LangDataDirectory =>
			Context.GetCPASettings().ApplyPathCapitalization("LangData/English/", PathCapitalizationType.All); // TODO

		public string LangDataLevelsDirectory => LangDataDirectory
			+ Context.GetCPASettings().ApplyPathCapitalization(DirectoryDescription?.GetDirectory(SNA_DescriptionType.DirectoryOfLevels).Substring("GameData/".Length), PathCapitalizationType.All);

		public virtual SNA_IDescription DirectoryDescription => GameDSB?.Value;

		public override string LevelsDirectory => GameDataDirectory 
			+ Context.GetCPASettings().ApplyPathCapitalization(DirectoryDescription?.GetDirectory(SNA_DescriptionType.DirectoryOfLevels), PathCapitalizationType.All);
		
		public string RelocationBigFilePath => Context.NormalizePath(LevelsDirectory, true) + "LEVELS0.DAT";

		public int? MapIndex => GameDSB?.Value?.GetMapIndex(Map);

		public override Dictionary<CPA_Path, string> GetPaths(string levelName) {
			var cpaSettings = Context.GetCPASettings();
			string ConvertCase(string path, PathCapitalizationType type) => cpaSettings.ApplyPathCapitalization(path, type);
			var gamedataDirectory = Context.NormalizePath(GameDataDirectory, true);
			var paths = new Dictionary<CPA_Path, string>();
			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				paths[CPA_Path.GameDSC] = $"{gamedataDirectory}{ConvertCase("gamedsc.bin", PathCapitalizationType.DSB)}";
			} else if (cpaSettings.EngineVersion == EngineVersion.TonicTroubleSE) {
				paths[CPA_Path.GameDSC] = $"{gamedataDirectory}{ConvertCase("GAME.DSC", PathCapitalizationType.DSB)}";
			} else {
				paths[CPA_Path.GameDSC] = $"{gamedataDirectory}{ConvertCase("Game.dsb", PathCapitalizationType.DSB)}";
			}
			if(DirectoryDescription == null) return paths;

			var levelsFolder = Context.NormalizePath(LevelsDirectory, true);

			// Prepare folder names
			string langDataPath = Context.NormalizePath(LangDataLevelsDirectory, true);

			paths[CPA_Path.RelocationBigFile] = levelsFolder + "LEVELS0.DAT";

			// Prepare paths
			paths[CPA_Path.FixSNA] = levelsFolder + ConvertCase("Fix.sna", PathCapitalizationType.Fix);
			paths[CPA_Path.FixRTB] = levelsFolder + ConvertCase("Fix.rtb", PathCapitalizationType.FixRelocation);
			paths[CPA_Path.FixGPT] = levelsFolder + ConvertCase("Fix.gpt", PathCapitalizationType.Fix);
			paths[CPA_Path.FixRTP] = levelsFolder + ConvertCase("Fix.rtp", PathCapitalizationType.FixRelocation);
			paths[CPA_Path.FixPTX] = levelsFolder + ConvertCase("Fix.ptx", PathCapitalizationType.Fix);
			paths[CPA_Path.FixRTT] = levelsFolder + ConvertCase("Fix.rtt", PathCapitalizationType.FixRelocation);
			paths[CPA_Path.FixSND] = levelsFolder + ConvertCase("Fix.snd", PathCapitalizationType.Fix);
			paths[CPA_Path.FixRTS] = levelsFolder + ConvertCase("Fix.rts", PathCapitalizationType.FixRelocation);

			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				paths[CPA_Path.FixSDA] = levelsFolder + ConvertCase("Fix.sda", PathCapitalizationType.Fix);
				paths[CPA_Path.FixLNG] = langDataPath + ConvertCase("Fix.lng", PathCapitalizationType.LangFix);
				paths[CPA_Path.FixRTG] = langDataPath + ConvertCase("Fix.rtg", PathCapitalizationType.FixRelocation);
				paths[CPA_Path.FixDLG] = langDataPath + ConvertCase("Fix.dlg", PathCapitalizationType.LangFix);
				paths[CPA_Path.FixRTD] = langDataPath + ConvertCase("Fix.rtd", PathCapitalizationType.FixRelocation);
			} else {
				paths[CPA_Path.FixSDA] = null;
				paths[CPA_Path.FixLNG] = null;
				paths[CPA_Path.FixRTG] = null;
				paths[CPA_Path.FixDLG] = null;
				paths[CPA_Path.FixRTD] = null;
			}


			// Prepare level folder names
			string lvlName = levelName;
			if(lvlName == null) return paths;
			string lvlFolder = ConvertCase($"{lvlName}/", PathCapitalizationType.LevelFolder);
			string langLvlFolder = ConvertCase($"{lvlName}/", PathCapitalizationType.LangLevelFolder);


			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				paths[CPA_Path.FixLevelRTB] = null;
			} else {
				paths[CPA_Path.FixLevelRTB] = levelsFolder + lvlFolder + ConvertCase("FixLvl.rtb", PathCapitalizationType.FixLvl);
			}
			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				paths[CPA_Path.FixLevelRTG] = langDataPath + langLvlFolder + ConvertCase("FixLvl.rtg", PathCapitalizationType.FixLvl);
			} else {
				paths[CPA_Path.FixLevelRTG] = null;
			}


			paths[CPA_Path.LevelSNA] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".sna", PathCapitalizationType.LevelFile);
			paths[CPA_Path.LevelGPT] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".gpt", PathCapitalizationType.LevelFile);
			paths[CPA_Path.LevelPTX] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".ptx", PathCapitalizationType.LevelFile);
			paths[CPA_Path.LevelSND] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".snd", PathCapitalizationType.LevelFile);
			if (RelocationBigFile == null) {
				paths[CPA_Path.LevelRTB] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtb", PathCapitalizationType.LevelRelocation);
				paths[CPA_Path.LevelRTP] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtp", PathCapitalizationType.LevelRelocation);
				paths[CPA_Path.LevelRTT] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtt", PathCapitalizationType.LevelRelocation);
				paths[CPA_Path.LevelRTS] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rts", PathCapitalizationType.LevelRelocation);
			} else {
				paths[CPA_Path.LevelRTB] = null;
				paths[CPA_Path.LevelRTP] = null;
				paths[CPA_Path.LevelRTT] = null;
				paths[CPA_Path.LevelRTS] = null;
			}
			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				paths[CPA_Path.LevelSDA] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".sda", PathCapitalizationType.LevelFile);
				paths[CPA_Path.LevelLNG] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".lng", PathCapitalizationType.LangLevelFile);
				paths[CPA_Path.LevelRTG] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".rtg", PathCapitalizationType.LangLevelFile);
				paths[CPA_Path.LevelDLG] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".dlg", PathCapitalizationType.LangLevelFile);
				paths[CPA_Path.LevelRTD] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".rtd", PathCapitalizationType.LangLevelRelocation);
			} else {
				paths[CPA_Path.LevelSDA] = null;
				paths[CPA_Path.LevelLNG] = null;
				paths[CPA_Path.LevelRTG] = null;
				paths[CPA_Path.LevelDLG] = null;
				paths[CPA_Path.LevelRTD] = null;
			}
			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				paths[CPA_Path.LevelDSC] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".dsb", PathCapitalizationType.DSB);
			} else {
				paths[CPA_Path.LevelDSC] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".dsc", PathCapitalizationType.DSB);
			}

			return paths;
		}
	}
}
