using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Raymap {
	// TODO: This will be the equivalent of the R2Loader
	public class CPA_SNAManager : LegacyGameManager {

		// TODO: Replace with GAMEDATA / GAMEDATABIN / ... later. This exists so we can use the top directory instead of GAMEDATA later on.
		public string GetGameDataFolder(CPA_Settings settings) => "/";

		public string GameDSCAlias => "game.dsc";

		protected override List<string> FindFiles(MapViewerSettings settings) {
			return base.FindFiles(settings);
			// TODO
		}

		public override async UniTask LoadFilesAsync(Context context) {
			Endian endian = context.GetCPASettings().GetEndian;
			var cpaSettings = context.GetCPASettings();
			string GameDataFolder = GetGameDataFolder(cpaSettings);

			// At this point we can only load the DSB file
			string gameDscPath;
			if (cpaSettings.EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				gameDscPath = Path.Combine(GameDataFolder, cpaSettings.ApplyPathCapitalization("gamedsc.bin", PathCapitalizationType.DSB));
			} else if (cpaSettings.EngineVersion == EngineVersion.TonicTroubleSE) {
				gameDscPath = Path.Combine(GameDataFolder, cpaSettings.ApplyPathCapitalization("GAME.DSC", PathCapitalizationType.DSB));
			} else {
				gameDscPath = Path.Combine(GameDataFolder, cpaSettings.ApplyPathCapitalization("Game.dsb", PathCapitalizationType.DSB));
			}
			var gameDsc = await context.AddLinearFileAsync(gameDscPath, endian);
			gameDsc.Alias = GameDSCAlias;
		}

		public async UniTask LoadDSC(Context context) {
			// The DSC file contains information about directories which we need to load the files
			GlobalLoadState.DetailedState = "Loading DSC";
			await TimeController.WaitIfNecessary();

			throw new NotImplementedException();
		}

		public async UniTask LoadFix(Context context) {
			GlobalLoadState.DetailedState = "Loading fix";
			await TimeController.WaitIfNecessary();

			throw new NotImplementedException();
		}

		public async UniTask LoadLevel(Context context, string levelName) {
			GlobalLoadState.DetailedState = "Loading level";
			await TimeController.WaitIfNecessary();

			throw new NotImplementedException();
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			// Load DSC
			await LoadDSC(context);

			// Load fix
			await LoadFix(context);

			// Load level
			await LoadLevel(context, context.GetMapViewerSettings().Map);

			throw new NotImplementedException();
		}
	}
}
