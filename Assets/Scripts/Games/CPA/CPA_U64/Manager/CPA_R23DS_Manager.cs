

using Cysharp.Threading.Tasks;

namespace Raymap {
	public class CPA_R23DS_Manager : CPA_U64Manager {

		public override async UniTask ExportTexturesAsync(MapViewerSettings settings, string outputDir) {
			await base.ExportTexturesAsync(settings, outputDir);

			// Stored separately
			/*for (int i = 1; i < 25; i++) {
				ExportEtcFile("LoadingAnimation/Course_" + i.ToString("D2") + ".etc", 64, 64, false);
			}
			ExportEtcFile("LoadingAnimation/home.etc", 64, 64, true);
			foreach (string file in Directory.EnumerateFiles(gameDataBinFolder + "/vignette")) {
				string fileName = file.Substring((gameDataBinFolder + "/vignette\\").Length);
				ExportEtcFile("vignette/" + fileName, 512, 256, false);
			}*/
		}
	}
}
