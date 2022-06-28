using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Raymap {
	public class CPA_TTPC_Manager : CPA_SNAManager {
		public override async UniTask LoadDSC(Context context) {
			// The DSC file contains information about directories which we need to load the files
			GlobalLoadState.DetailedState = "Loading DSC";
			await TimeController.WaitIfNecessary();

			SNA_File<SNA_Description_TT_Game> DSB = FileFactory.Read<SNA_File<SNA_Description_TT_Game>>(context, GameDSCAlias);

			throw new NotImplementedException();
		}
	}
}
