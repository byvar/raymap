using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using OpenSpace.PS1;
using UnityEngine;

namespace Raymap
{
	public class CPA_PS1Manager : LegacyGameManager
	{
		#region Paths

		//public string BINFilePath => "COMBIN.DAT";

		#endregion

		#region Game actions

		public override GameAction[] GetGameActions(MapViewerSettings settings) => new GameAction[]
		{
			new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output))
		};

		public async UniTask ExportBlocksAsync(MapViewerSettings settings, string outputDir)
		{
			throw new NotImplementedException();

			Debug.Log("Finished exporting blocks");
		}

		#endregion

		#region Manager Methods

		protected override List<string> FindFiles(MapViewerSettings settings)
		{
			// TODO: Re-implement
			return PS1GameInfo.Games[GetLegacyMode(settings)].maps.ToList();
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context)
		{
			GlobalLoadState.DetailedState = $"Loading level";
			await TimeController.WaitIfNecessary();


			throw new NotImplementedException();
		}

		public override async UniTask LoadFilesAsync(Context context)
		{
			Endian endian = context.GetCPASettings().GetEndian;

		}

		#endregion
	}
}
