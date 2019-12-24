using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	public class ROMAnimation : OpenSpaceStruct {
		public bool compressed;
		public uint index;
		public uint compressedSize;
		public byte[] data;

		protected override void ReadInternal(Reader reader) {
			byte[] compressedBytes = reader.ReadBytes((int)compressedSize);
			if (compressed) {
				PeepsCompress.YAY0 yay0 = new PeepsCompress.YAY0();
				using (MemoryStream ms = new MemoryStream(compressedBytes)) {
					using (Reader r = new Reader(ms, false)) {
						byte[] decompressed = yay0.decompress(r);
						data = decompressed;
					}
				}
			} else {
				data = compressedBytes;
			}
			/*if (data != null && data.Length > 0) {
				Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "exported_anims/anim_" + index + ".bin", data);
			}*/
		}
	}
}
