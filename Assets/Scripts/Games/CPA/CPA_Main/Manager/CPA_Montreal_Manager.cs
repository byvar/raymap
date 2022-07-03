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

			var cpaGlobals = (CPA_Globals_Montreal)context.GetCPAGlobals();
			cpaGlobals.GameDSB_Montreal = FileFactory.Read<SNA_File<SNA_Description_Montreal_Game>>(context, CPA_Path.GameDSC.ToString());

			// Test:
			//await context.AddLinearFileAsync($"World/Levels/{context.GetMapViewerSettings().Map}/{context.GetMapViewerSettings().Map}pgb.bin");
			//SNA_File <SNA_Description_Montreal_LevelPGB> LevelPGB = FileFactory.Read<SNA_File<SNA_Description_Montreal_LevelPGB>>(context, $"World/Levels/{context.GetMapViewerSettings().Map}/{context.GetMapViewerSettings().Map}pgb.bin");
		}
		public override void InitGlobals(Context context) => new CPA_Globals_Montreal(context);
	}
}
