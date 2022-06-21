

using Cysharp.Threading.Tasks;

namespace Raymap {
	public class CPA_R2DS_Manager : CPA_U64Manager {

		public override async UniTask ExportTexturesAsync(MapViewerSettings settings, string outputDir) {
			await base.ExportTexturesAsync(settings, outputDir);

			/*PAL palette = new PAL(gameDataBinFolder + "hud/objpal.bin");
			ExportNBFC("hud/sprlums1.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/sprcage1.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/sprraym1.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/sprraym2.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/sprenmy1.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/sprnumb0.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb1.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb2.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb3.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb4.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb5.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb6.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb7.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb8.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprnumb9.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/sprslash.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/spbars.nbfc", 1, 9, palette.palette);
			ExportNBFC("hud/stick.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/stickbase.nbfc", 8, 8, palette.palette);
			ExportNBFC("hud/sprspark.nbfc", 1, 1, palette.palette);
			ExportNBFC("hud/ok.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/smaller.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/bigger.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/starhi.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/starmed.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/starlow.nbfc", 2, 2, palette.palette);
			ExportNBFC("hud/slider.nbfc", 4, 4, palette.palette);
			ExportNBFC("hud/slider.nbfc", 4, 4, palette.palette);
			ExportGFX("hud/bgcalib.gfx", "hud/bgcalib.map", "hud/bgcalib.pal", 32, 32); // tiles: 768
			ExportGFX("hud/mainbg.gfx", "hud/mainbg.map", "hud/mainbg.pal", 32, 32); // tiles: 864*/
		}
	}
}
