using BinarySerializer;
using BinarySerializer.Audio.RIFF;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Raymap {
	// TODO: This will be the equivalent of the R3Loader
	public class CPA_LVLManager : LegacyGameManager {

		#region Game actions
		public override GameAction[] GetGameActions(MapViewerSettings settings) => new GameAction[] {
			new GameAction("Export Audio", true, true, (input, output) => ExportAudioAsync(settings, input, output)),
		};
		public async UniTask ExportAudioAsync(MapViewerSettings settings, string inputDir, string outputDir) {
			using (var context = new MapViewerContext(inputDir, settings)) {
				// Get the deserializer
				var s = context.Deserializer;
				s.Context.AddSettings<RIFFSettings>(new RIFFSettings());
				RIFFSettings riff = s.GetRequiredSettings<RIFFSettings>();
				riff.RegisterWAV();
				riff.RegisterUSND();
				riff.AlignChunks = false;

				await LoadFilesAsync(context);

				Dictionary<USND_UUID, USND_IdObjInfo> infos = new Dictionary<USND_UUID, USND_IdObjInfo>();
				Dictionary<USND_UUID, string> names = new Dictionary<USND_UUID, string>();

				foreach (string file in Directory.EnumerateFiles(context.BasePath, "*.hx2", SearchOption.AllDirectories)) {
					string fileName = file.Substring(context.BasePath.Length);
					var linf = await context.AddLinearFileAsync(fileName, endianness: context.GetCPASettings().GetEndian);
					var soundbank = FileFactory.Read<USND_SoundBank>(context, fileName);
					foreach (var i in soundbank.IdObjInfos) {
						if(!infos.ContainsKey(i.Id))
							infos[i.Id] = i;
					}
				}
				UnityEngine.Debug.Log("Finished exporting audio");
				return;
				const string bnhPattern = @"^cuuid\( 0x(?<hw>........), 0x(?<lw>........) \).*file (?<name>.*)";
				foreach (string file in Directory.EnumerateFiles(context.BasePath, "*.bnh", SearchOption.AllDirectories)) {
					var lines = File.ReadAllLines(file, System.Text.Encoding.UTF8);
					foreach (var l in lines) {
						Match bnhMatch = Regex.Match(l, bnhPattern);
						if (bnhMatch.Success) {
							var name = bnhMatch.Groups["name"].Value;
							if (!string.IsNullOrWhiteSpace(name)) {
								var hw = uint.Parse(bnhMatch.Groups["hw"].Value, System.Globalization.NumberStyles.HexNumber);
								var lw = uint.Parse(bnhMatch.Groups["lw"].Value, System.Globalization.NumberStyles.HexNumber);
								var cuuid = new USND_UUID() { HighWord = hw, LowWord = lw };
								context.SystemLogger?.LogInfo($"{cuuid} - {name}");
								if (!names.ContainsKey(cuuid))
									names[cuuid] = name;
							}
						}
					}
				}

				foreach (var info in infos) {
					// Fill in names
					if (info.Value.IdObj is USND_WavResData res) {
						if (string.IsNullOrWhiteSpace(res.Name) && names.ContainsKey(res.Id)) {
							res.Name = names[res.Id];
						}
						if (!string.IsNullOrWhiteSpace(res.Name)) {
							if (res.LocalizedFileIds?.Any() ?? false) {
								foreach (var loc in res.LocalizedFileIds) {
									if (!names.ContainsKey(loc.FileId)) {
										var langCode = loc.LanguageCode;
										if (loc.Context.GetCPASettings().GetEndian == Endian.Little) {
											var langCodeChars = langCode.ToCharArray();
											Array.Reverse(langCodeChars);
											langCode = new string(langCodeChars);
										}
										langCode = langCode.Trim();
										names[loc.FileId] = $"{res.Name}_{langCode}";
									}
								}
							}
						}
					}
				}
				foreach (var info in infos) {
					// Fill in names
					if (info.Value.IdObj is USND_WavResData res) {
						if (!string.IsNullOrWhiteSpace(res.Name)) {
							if (!names.ContainsKey(res.FileId)) {
								names[res.FileId] = res.Name;
							}
						}
					}
				}
				foreach (var info in infos) {
					// Export
					if (info.Value.IdObj is USND_WaveFileIdObj obj) {
						var name = names[obj.Id];
						var extension = "lol";
						//Util.ByteArrayToFile(Path.Combine(outputDir, $"{name}.{extension}"), obj.WaveFile.WaveFile);
					}
				}
			}

			UnityEngine.Debug.Log("Finished exporting audio");
		}
		#endregion

		protected override List<string> FindFiles(MapViewerSettings settings) {
			return base.FindFiles(settings);
			// TODO
		}

	}
}
