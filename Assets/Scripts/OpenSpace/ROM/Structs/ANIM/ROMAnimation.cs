using OpenSpace.ROM.ANIM.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	// Data in anims.bin
	public class ROMAnimation : OpenSpaceStruct {
		public bool compressed;
		public uint index;
		public uint compressedSize;
		public byte[] data;

		public AnimA3DGeneral a3d;
		public AnimVector[] vectors;
		public AnimQuaternion[] quaternions;
		public AnimHierarchy[] hierarchies;
		public AnimNTTO[] ntto;
		public AnimNumOfNTTO[] numOfNTTO;
		public AnimOnlyFrame[] onlyFrames;
		public AnimChannel[] channels;
		public AnimKeyframe[] keyframes;
		public AnimEvent[] events;
		public AnimMorphData[] morphData;

		private bool dataParsed = false;

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
			//ReadData();
		}

		public void ReadData() {
			if (!dataParsed) {
				dataParsed = true;
				if (data != null && data.Length > 0) {
					//Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "exported_anims/anim_" + index + ".bin", data);
					using (MemoryStream ms = new MemoryStream(data)) {
						using (Reader r = new Reader(ms, Settings.s.IsLittleEndian)) {
							a3d = new AnimA3DGeneral(r);
							vectors = new AnimVector[a3d.num_vectors];
							quaternions = new AnimQuaternion[a3d.num_quaternions];
							hierarchies = new AnimHierarchy[a3d.num_hierarchies];
							ntto = new AnimNTTO[a3d.num_NTTO];
							onlyFrames = new AnimOnlyFrame[a3d.this_num_onlyFrames - a3d.this_start_onlyFrames];
							channels = new AnimChannel[a3d.num_channels];
							numOfNTTO = new AnimNumOfNTTO[a3d.num_channels * a3d.num_numNTTO];
							keyframes = new AnimKeyframe[a3d.num_keyframes];
							events = new AnimEvent[a3d.num_events];
							morphData = new AnimMorphData[a3d.num_morphData];

							for (int i = 0; i < vectors.Length; i++) vectors[i] = new AnimVector(r);
							for (int i = 0; i < quaternions.Length; i++) quaternions[i] = new AnimQuaternion(r);
							for (int i = 0; i < hierarchies.Length; i++) hierarchies[i] = new AnimHierarchy(r);
							for (int i = 0; i < ntto.Length; i++) ntto[i] = new AnimNTTO(r);
							for (int i = 0; i < onlyFrames.Length; i++) onlyFrames[i] = new AnimOnlyFrame(r);
							for (int i = 0; i < channels.Length; i++) channels[i] = new AnimChannel(r);
							for (int i = 0; i < numOfNTTO.Length; i++) numOfNTTO[i] = new AnimNumOfNTTO(r);
							for (int i = 0; i < keyframes.Length; i++) keyframes[i] = new AnimKeyframe(r);
							r.Align(4);
							for (int i = 0; i < events.Length; i++) events[i] = new AnimEvent(r);
							for (int i = 0; i < morphData.Length; i++) morphData[i] = new AnimMorphData(r);
							if (r.BaseStream.Position != r.BaseStream.Length) {
								MapLoader.Loader.print("Animation " + index + ": bad read");
							}
						}
					}
				}
			}
		}
	}
}
