using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Raymap {
	public class CPA_Montreal_Manager : CPA_SNAManager {
		public override async UniTask LoadDSC(Context context) {
			// The DSC file contains information about directories which we need to load the files
			GlobalLoadState.DetailedState = "Loading DSC";
			await TimeController.WaitIfNecessary();

			SNA_File<SNA_Description_Montreal_Game> DSB = FileFactory.Read<SNA_File<SNA_Description_Montreal_Game>>(context, GameDSCAlias);

			// Test:
			//await context.AddLinearFileAsync($"World/Levels/{context.GetMapViewerSettings().Map}/{context.GetMapViewerSettings().Map}pgb.bin");
			//SNA_File <SNA_Description_Montreal_LevelPGB> LevelPGB = FileFactory.Read<SNA_File<SNA_Description_Montreal_LevelPGB>>(context, $"World/Levels/{context.GetMapViewerSettings().Map}/{context.GetMapViewerSettings().Map}pgb.bin");
			throw new NotImplementedException();
		}
	}
}
