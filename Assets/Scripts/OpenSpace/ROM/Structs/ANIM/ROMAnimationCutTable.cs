using OpenSpace.Loader;
using OpenSpace.ROM.ANIM.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	// Data in cuttable.bin
	public class ROMAnimationCutTable : OpenSpaceStruct {
		public ushort length;
		public AnimationCut[] entries;


		protected override void ReadInternal(Reader reader) {
			entries = new AnimationCut[length];
			for (ushort i = 0; i < length; i++) {
				entries[i] = new AnimationCut(reader, i);
			}
		}

		public void ReadData(ushort index) {
			if (index != 0xFFFF && index < length) {
				R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
				ushort currentIndex = index;
				while (currentIndex != 0xFFFF) {
					l.romAnims[currentIndex].ReadData();
					currentIndex = entries[currentIndex].nextAnim;
				}
			}
		}

		public AnimationCut[] GetAnimationChain(ushort index) {
			if (index != 0xFFFF && index < length) {
				List<AnimationCut> cuts = new List<AnimationCut>();
				ushort currentIndex = index;
				while (currentIndex != 0xFFFF) {
					cuts.Add(entries[currentIndex]);
					currentIndex = entries[currentIndex].nextAnim;
				}
				return cuts.ToArray();
			}
			return null;
		}

		public struct AnimationCut {
			public ushort startFrame;
			public ushort currentTotalFrames;
			public ushort previousAnim;
			public ushort nextAnim;
			public ushort word8;
			public ushort wordA; // always 0
			public ushort index;

			public AnimationCut(Reader reader, ushort index) {
				startFrame = reader.ReadUInt16();
				currentTotalFrames = reader.ReadUInt16();
				previousAnim = reader.ReadUInt16();
				nextAnim = reader.ReadUInt16();
				word8 = reader.ReadUInt16();
				wordA = reader.ReadUInt16();
				this.index = index;
			}
		}
	}
}
