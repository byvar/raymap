﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
    public class LanguageParser {
		public static string ReadNullDelimitedString(BinaryReader r, Encoding encoding) {
			List<byte> bytes = new();
			byte b = r.ReadByte();

			while (b != 0x0) {
				bytes.Add(b);
				b = r.ReadByte();
			}

			if (bytes.Count > 0) {
				if (encoding == null)
					throw new ArgumentNullException(nameof(encoding));
				return encoding.GetString(bytes.ToArray());
			}

			return String.Empty;
		}
		public static string ReadSpecialEncodedString(BinaryReader r, CPA_GameMode mode, VersionLanguage lang = VersionLanguage.Japanese, bool includeFormat = false) {
			Encoding wind1252 = Encoding.GetEncoding(1252);
			Encoding wind1255 = lang == VersionLanguage.Hebrew ? Encoding.GetEncoding(1255) : null;
			int curObjectTableOffset = 0;

			if (mode == CPA_GameMode.Rayman2IOS) {
				return ReadNullDelimitedString(r, Encoding.UTF8);
			}

			bool isEnd = false;
			StringBuilder build = new StringBuilder();
			List<byte> formatBytes = new List<byte>();
			while (!isEnd) {
				byte b = r.ReadByte();
				if (b == '/') {
					formatBytes.Add(b);
					while (b != ':' && !isEnd) {
						b = r.ReadByte();
						if (b == '\0') isEnd = true;
						if (!isEnd) formatBytes.Add(b);
					}
					var formatString = wind1252.GetString(formatBytes.ToArray());
					if (includeFormat) {
						build.Append(formatString);
					}
					formatString = formatString.ToLowerInvariant();
					if (formatString.StartsWith("/o")) {
						var formatCheck = Regex.Match(formatString, @"/o(?<number>([0-9]+)):");
						if (formatCheck.Success) {
							var newOffset = formatCheck.Groups["number"].Value;
							if (int.TryParse(newOffset, out int newOffsetNumber)) {
								curObjectTableOffset = newOffsetNumber;
							}
						}
					}
					formatBytes.Clear();
				} else if(b == '\0') {
					isEnd = true;
				} else {
					if (lang == VersionLanguage.Japanese) {
						if (mode == CPA_GameMode.Rayman2DC) {
							if (b < 32) continue;
							if (b == 32) {
								build.Append(' ');
								continue;
								//b -= 32;
							} else {
								b -= 33;
							}
							var newStr = JapaneseR2DC[b];
							if (newStr != null) {
								build.Append(newStr);
							} else {
								build.Append($"[BYTE_{b}_{curObjectTableOffset}]");
							}
						} else if (mode == CPA_GameMode.Rayman2PS2) {
							if (b < 32) continue;
							int objectTableIndex = b - 32;
							if (objectTableIndex == 0) {
								build.Append(' ');
								continue;
							}
							if (curObjectTableOffset == 800)
								objectTableIndex += curObjectTableOffset;
							var newStr = JapaneseR2PS2[objectTableIndex];
							if (newStr != null) {
								build.Append(newStr);
							} else {
								build.Append($"[BYTE_{b}_{curObjectTableOffset}]");
							}
						}
					} else if (lang == VersionLanguage.Hebrew) {
						if (b < 0xE0)
							build.Append(wind1252.GetChars(new byte[] { b }));
						else
							build.Append(wind1255.GetChars(new byte[] { b }));
					}
				}
			}
			return build.ToString();
		}

		public static string[] JapaneseR2DC = new string[] {
			"そ",
			"′",
			"せ",
			"す",
			"し",
			"さ",
			"こ",
			null,
			null,
			"き",
			"か",
			"あ",
			null,
			"お",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"え",
			"い",
			"い",
			null,
			"う",
			null,
			null,
			"く",
			"け",
			null,
			"９",
			"８",
			"７",
			"６",
			"５",
			"４",
			"３",
			"２",
			"１",
			"０",
			"▶",
			"、",
			"ー",
			"／",
			"・",
			"％",
			"[Button_-]",
			"[AnalogStick]",
			"[Button_S]",
			"[Button_R]",
			"[Button_L]",
			"↑",
			"←",
			"→",
			"↓",
			"[Button_+]",
			"[DPad]",
			"[Button_X]",
			"[Button_Y]",
			"[Button_B]",
			"[Button_A]",
			"。",
			"？",
			"\"",
			"↓",
			"↑",
			"↓",
			"〜",
			"！",
			"：",
			"，",
			null,
			"ッ",
			"ョ",
			"ュ",
			"ャ",
			"ォ",
			"ェ",
			"ゥ",
			"ィ",
			"ァ",
			"ポ",
			"ペ",
			"プ",
			"ピ",
			"パ",
			"ボ",
			"ベ",
			"ブ",
			"ビ",
			"バ",
			"ド",
			"デ",
			"ヅ",
			"ヂ",
			"ダ",
			"ゾ",
			"ゼ",
			"ズ",
			"ジ",
			"ザ",
			"ゴ",
			"ゲ",
			"グ",
			"ギ",
			"ガ",
			"ン",
			"ヲ",
			"ワ",
			"ロ",
			"レ",
			"ル",
			"リ",
			"ラ",
			"ヨ",
			"ユ",
			"ヤ",
			"モ",
			"メ",
			"ム",
			"ミ",
			"マ",
			"ホ",
			"ヘ",
			"フ",
			"ヒ",
			"ハ",
			"ノ",
			"ネ",
			"ヌ",
			"ニ",
			"ナ",
			"ト",
			"テ",
			"ツ",
			"チ",
			"タ",
			"ソ",
			"セ",
			"ス",
			"シ",
			"サ",
			"コ",
			"ケ",
			"ク",
			"キ",
			"カ",
			"オ",
			"エ",
			"ウ",
			"イ",
			"ア",
			"っ",
			"ょ",
			"ゅ",
			"ゃ",
			"ぉ",
			"ぇ",
			"ぅ",
			"ぃ",
			"ぁ",
			"ぽ",
			"ぺ",
			"ぷ",
			"ぴ",
			"ぱ",
			"ぼ",
			"べ",
			"ぶ",
			"び",
			"ば",
			"ど",
			"で",
			"づ",
			"ぢ",
			"だ",
			"ぞ",
			"ぜ",
			"ず",
			"じ",
			"ざ",
			"ご",
			"げ",
			"ぐ",
			"ぎ",
			"が",
			"ん",
			"を",
			"わ",
			"ろ",
			"れ",
			"る",
			"り",
			"ら",
			"よ",
			"ゆ",
			"や",
			"も",
			"め",
			"む",
			"み",
			"ま",
			"ほ",
			"へ",
			"ふ",
			"ひ",
			"は",
			"の",
			"ね",
			"ぬ",
			"に",
			"な",
			"と",
			"て",
			"つ",
			"ち",
			"た",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
		};

		public static string[] JapaneseR2PS2 = new string[] {
			" ",
			"！",
			null,
			null,
			"あ",
			"％",
			"ア",
			"′",
			null,
			null,
			"″",
			"＋",
			"，",
			"－",
			"ば",
			null,
			"０",
			"１",
			"２",
			"３",
			"４",
			"５",
			"６",
			"７",
			"８",
			"９",
			null,
			"／",
			null,
			"：",
			null,
			"？",
			null,
			"Ａ",
			"Ｂ",
			"Ｃ",
			"Ｄ",
			"Ｅ",
			"Ｆ",
			"Ｇ",
			"Ｈ",
			"Ｉ",
			"Ｊ",
			"Ｋ",
			"Ｌ",
			"Ｍ",
			"Ｎ",
			"Ｏ",
			"Ｐ",
			"Ｑ",
			"Ｒ",
			"Ｓ",
			"Ｔ",
			"Ｕ",
			"Ｖ",
			"Ｗ",
			"Ｘ",
			"Ｙ",
			"Ｚ",
			"バ",
			"べ",
			"ベ",
			"び",
			"ビ",
			"ぼ",
			"ボ",
			"ぶ",
			"ブ",
			"だ",
			"ダ",
			"で",
			"デ",
			"ど",
			"ド",
			"づ",
			"え",
			"エ",
			"が",
			"ガ",
			"げ",
			"ゲ",
			"ぎ",
			"ギ",
			"ご",
			"ゴ",
			"ぐ",
			"グ",
			"は",
			"ハ",
			"へ",
			"ヘ",
			"ひ",
			"ほ",
			"ホ",
			"ふ",
			"フ",
			"い",
			"イ",
			"か",
			"カ",
			"け",
			"ケ",
			"き",
			"キ",
			"こ",
			"コ",
			"く",
			"ク",
			"ま",
			"マ",
			"め",
			"メ",
			"み",
			"ミ",
			"も",
			"モ",
			"む",
			"ム",
			"な",
			"ナ",
			"ね",
			"ネ",
			"に",
			"ニ",
			"の",
			"ノ",
			"ぬ",
			"ん",
			"ン",
			"お",
			"オ",
			"ぱ",
			"パ",
			"ぺ",
			"ピ",
			"ぽ",
			"ポ",
			"プ",
			"ら",
			"ラ",
			"れ",
			"レ",
			"り",
			"リ",
			"ろ",
			"ロ",
			"る",
			"ル",
			"さ",
			"サ",
			"せ",
			"セ",
			"し",
			"シ",
			"そ",
			"ソ",
			"す",
			"ス",
			"た",
			"タ",
			"て",
			"テ",
			"ち",
			"チ",
			"と",
			"ト",
			"つ",
			"ツ",
			"う",
			"ウ",
			"ヴ",
			"わ",
			"ワ",
			"を",
			"や",
			"ヤ",
			"よ",
			"ゆ",
			"ざ",
			"ザ",
			"ぜ",
			"ゼ",
			"じ",
			"ジ",
			"ぞ",
			"ず",
			"ズ",
			"ぁ",
			"ァ",
			"ぇ",
			"ェ",
			"ィ",
			"ォ",
			"（",
			"）",
			"・",
			null,
			"ー",
			"、",
			"っ",
			null,
			null,
			"ッ",
			"ゃ",
			"ャ",
			"ょ",
			"ョ",
			"ゅ",
			"ュ",
			"。",
			"～",
		}
			.Concat(Enumerable.Repeat((string)null, 817 - 211))
			.Concat(new string[] {
				"[Button_Cross]",
				"[Button_Circle]",
				"[Button_Triangle]",
				"[Button_Square]",
				"[Button_L1]",
				"[Button_L2]",
				"[Button_R1]",
				"[Button_R2]",
				"[Button_Start]",
			})
			.Concat(Enumerable.Repeat((string)null, 865 - 826))
			.Concat(new string[] {
				"[Button_Sel",
				null,
				"ect]",
			})
			.Concat(Enumerable.Repeat((string)null, 873 - 868))
			.Concat(new string[] {
				"[AnalogStick1]",
				"[AnalogStick2]",
			})
			.ToArray();
	}
}