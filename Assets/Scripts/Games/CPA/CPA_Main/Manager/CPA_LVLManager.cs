using BinarySerializer;
using BinarySerializer.Audio;
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
				if (s.Context.GetCPASettings().Platform == Platform.PS2) {
					riff.AlignChunks = false;
				}

				await LoadFilesAsync(context);

				Dictionary<USND_UUID, USND_IdObjInfo> infos = new Dictionary<USND_UUID, USND_IdObjInfo>();
				Dictionary<USND_UUID, string> names = new Dictionary<USND_UUID, string>();

				string[] hxExtensions = { "*.hx2", "*.hxc" };

				foreach (var hxExtension in hxExtensions) {
					foreach (string file in Directory.EnumerateFiles(context.BasePath, hxExtension, SearchOption.AllDirectories)) {
						string fileName = file.Substring(context.BasePath.Length);
						var linf = await context.AddLinearFileAsync(fileName, endianness: context.GetCPASettings().GetEndian);
						var soundbank = FileFactory.Read<USND_SoundBank>(context, fileName);
						foreach (var i in soundbank.IdObjInfos) {
							if (!infos.ContainsKey(i.Id))
								infos[i.Id] = i;
						}
					}
				}

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
								//context.SystemLogger?.LogInfo($"{cuuid} - {name}");
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
						// Only external supported for now...
						if (!obj.Flags.HasFlag(USND_WaveFileFlags.External)) continue;
						var ogWav = obj.WaveFile.WaveFile;
						var format = ogWav.Format;

						// codec, from https://github.com/vgmstream/vgmstream/blob/master/src/meta/ubi_hx.c
						var name = names[obj.Id];
						var codec = format.FormatType switch {
							0x01=> "PCM",
							0x02=> "UBI",
							0x03=> "PSX",
							0x04=> "DSP",
							0x05=> "XIMA",
							0x55=> "MP3",
							_ => throw new Exception("Unknown format!")
						};
						var extension = codec switch {
							"PCM" => "wav",
							"UBI" => "mib", // just random extension since we'll use txth
							"PSX" => "mib",
							"DSP" => "dsp",
							"XIMA" => "xima",
							"ATRAC3" => "at3",
							"XMA2" => "xwav",
							"MP3" => "mp3",
							_ => "mib"
						};
						var outputFilePath = Path.Combine(outputDir, $"{name}.{extension}");
						var txthPath = Path.Combine(outputDir, $"{name}.{extension}.txth");

						var absolutePath = obj.Offset.File.AbsolutePath;
						var externalFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(absolutePath), obj.ExternalFileName);
						var datX = ogWav.Riff.GetRequiredChunk<RIFF_Chunk_DatX>();
						byte[] data = new byte[datX.SizeInExternalFile];

						using (BinaryReader reader = new BinaryReader(new FileStream(externalFilePath, FileMode.Open))) {
							reader.BaseStream.Seek(datX.OffsetInExternalFile, SeekOrigin.Begin);
							reader.Read(data, 0, data.Length);
						}

						// Create and open the output file
						using (var outputStream = File.Create(outputFilePath)) {
							// Create a context
							using (var wavContext = new MapViewerContext(settings, log: false)) {
								// Create a key
								const string wavKey = "wav";
								wavContext.AddSettings<RIFFSettings>(new RIFFSettings());
								RIFFSettings riffOutput = wavContext.GetRequiredSettings<RIFFSettings>();
								riffOutput.AlignChunks = true;
								riffOutput.RegisterWAV();

								// Add the file to the context
								wavContext.AddFile(new StreamFile(wavContext, wavKey, outputStream));

								var wav = new WAV();
								List<RIFF_Chunk> chunks = new List<RIFF_Chunk>();
								chunks.Add(ogWav.Format.CreateChunk());
								foreach (var ck in ogWav.Riff.Chunks) {
									switch (ck.Data) {
										case RIFF_Chunk_Label:
										case RIFF_Chunk_LabeledText:
										case RIFF_Chunk_List:
										case RIFF_Chunk_Cue:
											chunks.Add(ck.Data.CreateChunk());
											break;
									}
								}
								// TODO: add label stuff. needs to be converted to labl and cue chunks...
								chunks.Add(new RIFF_Chunk_Data() {
									Data = data
								}.CreateChunk());

								wav.Riff.Chunks = chunks.ToArray();

								// Write the data
								FileFactory.Write<WAV>(wavContext, wavKey, wav);

								if (codec == "PSX" || codec == "UBI") {
									var headerSkip = 0;
									if (codec == "PSX") {
										headerSkip = (0x10 * wav.Format.ChannelCount);
										// arguably also skip the last 0x10 * channelcount as it looks like garbage data
									}
									string[] txthContent = {
										$"codec = {codec}",
										$"sample_rate = {wav.Format.SampleRate}",
										$"channels = {wav.Format.ChannelCount}",
										$"start_offset = {wav.Data.Offset.FileOffset + headerSkip}",
										"num_samples = data_size"
									};
									File.WriteAllLines(txthPath, txthContent);
								}
							}
						}
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
