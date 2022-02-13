using UnityEngine;
using UnityEditor;

namespace OpenSpace.Text {
    public class FontStructure : OpenSpaceStruct {
		public byte num_fontsBitmap;
		public byte num_fonts;
		public FontDefine[] fonts;

		// Only on Xbox 360 maybe for special languages?
		public byte num_fontsBitmap2;
		public byte num_fonts2;
		public FontDefine[] fonts2;

		public LegacyPointer[] fontsBitmap;

		protected override void ReadInternal(Reader reader) {
			// 1 fontDefine is normally sized 0x12E4 ( 0x12B0 if no names )
			num_fontsBitmap = reader.ReadByte();
			num_fonts = reader.ReadByte();
			fonts = new FontDefine[num_fonts];
			fontsBitmap = new LegacyPointer[num_fontsBitmap];
			if (CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) {
				for (int i = 0; i < num_fonts; i++) {
					fonts[i] = new FontDefine(reader);
				}
				if (CPA_Settings.s.platform == CPA_Settings.Platform.Xbox360 || CPA_Settings.s.platform == CPA_Settings.Platform.PS3) {
					reader.Align(4); // Align position
					num_fontsBitmap2 = reader.ReadByte();
					num_fonts2 = reader.ReadByte();
					fonts2 = new FontDefine[num_fonts2];
					for (int i = 0; i < num_fonts2; i++) {
						fonts2[i] = new FontDefine(reader);
					}
				}
				reader.Align(4); // Align position
				for (int i = 0; i < num_fontsBitmap; i++) {
					fontsBitmap[i] = LegacyPointer.Read(reader);
				}
			} else {
				// Largo Winch
				reader.Align(4); // Align position
				for (int i = 0; i < num_fontsBitmap; i++) {
					fontsBitmap[i] = LegacyPointer.Read(reader);
				}
				LegacyPointer off_fontDefine = LegacyPointer.Read(reader);
				LegacyPointer.DoAt(ref reader, off_fontDefine, () => {
					for (int i = 0; i < num_fonts; i++) {
						fonts[i] = new FontDefine(reader);
					}
				});
			}
		}


		public class CharacterDefine {
			public ushort x;
			public ushort y;
			public ushort height;
			public ushort width;
			public ushort pixelSize;
			public ushort unk_0A;
			public byte ind_fontBitmap;
			public byte isDefined;

			public CharacterDefine(Reader reader) {
				x = reader.ReadUInt16();
				y = reader.ReadUInt16();
				height = reader.ReadUInt16();
				width = reader.ReadUInt16();
				if (CPA_Settings.s.game != CPA_Settings.Game.Dinosaur) {
					if (CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) {
						pixelSize = reader.ReadUInt16();
					}
					unk_0A = reader.ReadUInt16();
				}
				ind_fontBitmap = reader.ReadByte();
				isDefined = reader.ReadByte();
			}
		}
		public struct KerningInfo {
			public ushort previousCharacter;
			public ushort currentCharacter;
			public ushort kerningPixels;

			public KerningInfo(Reader reader) {
				previousCharacter = reader.ReadUInt16();
				currentCharacter = reader.ReadUInt16();
				kerningPixels = reader.ReadUInt16();
			}
		}
		public class FontDefine {
			public string name;
			public CharacterDefine[] characters;
			public KerningInfo[] kerningInfo;
			public ushort unk;

			public FontDefine(Reader reader) {
				if (CPA_Settings.s.platform == CPA_Settings.Platform.GC
					|| CPA_Settings.s.platform == CPA_Settings.Platform.Xbox360
					|| CPA_Settings.s.platform == CPA_Settings.Platform.PS3
					|| CPA_Settings.s.mode == CPA_Settings.Mode.RaymanArenaXbox) {
					name = reader.ReadString(50);
				}
				characters = new CharacterDefine[256];
				for (int i = 0; i < characters.Length; i++) {
					characters[i] = new CharacterDefine(reader);
				}
				if (CPA_Settings.s.game != CPA_Settings.Game.Dinosaur && CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) {
					kerningInfo = new KerningInfo[200];
					for (int i = 0; i < kerningInfo.Length; i++) {
						kerningInfo[i] = new KerningInfo(reader);
					}
					unk = reader.ReadUInt16();
				}
			}
		}
	}
}