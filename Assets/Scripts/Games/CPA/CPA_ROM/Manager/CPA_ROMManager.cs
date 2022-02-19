using BinarySerializer;
using BinarySerializer.Ubisoft.CPA;
using BinarySerializer.Ubisoft.CPA.ROM;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using OpenSpace;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymap {
	public class CPA_ROMManager : LegacyGameManager {
		public string AnimsFilePath => "anims.bin";
		public string ShortAnimsFilePath => "shAnims.bin";
		public string AnimsCutTableFilePath => "cuttable.bin";

		public override async UniTask LoadFilesAsync(Context context) {
			Endian endian = context.GetCPASettings().GetEndian;
			await context.AddLinearFileAsync(AnimsFilePath, endianness: endian);
			await context.AddLinearFileAsync(ShortAnimsFilePath, endianness: endian);
			await context.AddLinearFileAsync(AnimsCutTableFilePath, endianness: endian);
		}

		public async UniTask LoadAnimations(Context context) {
			var animsFile = FileFactory.Read<A3D_AnimationsFile>(context, AnimsFilePath);
			for (int i = 0; i < animsFile.AnimationsCount; i++) {
				if (i % 16 != 0) continue; // This is just a test
				if (i % 256 == 0) {
					GlobalLoadState.DetailedState = $"Loading animations: anims ({i + 1}/{animsFile.AnimationsCount})";
					await TimeController.WaitIfNecessary();
				}
				animsFile.LoadAnimation(context.Deserializer, i);
			}
			var shAnimsFile = FileFactory.Read<A3D_ShortAnimationsFile>(context, ShortAnimsFilePath);
			var cutTableFile = FileFactory.Read<A3D_AnimationCutTable>(context, AnimsCutTableFilePath);
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) {
			// Load animations
			await LoadAnimations(context);

			throw new NotImplementedException();
		}
	}
}
