using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class PersoMatrixAndVector : ROMStruct {
		// Size: 6
		public ushort matrixIndex;
		public ushort vectorIndex;
		public ushort flags;

		public ROMTransform.ROMMatrix matrix;
		public Vector3? vector;


		protected override void ReadInternal(Reader reader) {
			matrixIndex = reader.ReadUInt16();
			vectorIndex = reader.ReadUInt16();
			flags = reader.ReadUInt16();

			ParseMatrixAndVector();
		}
		private void ParseMatrixAndVector() {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			LevelHeader lh = l.Get<LevelHeader>((ushort)(l.CurrentLevel | (ushort)FATEntry.Flag.Fix));
			if (lh != null) {
				Short3Array indices = lh.indices.Value;
				Vector3Array vectors = lh.vectors.Value;
				matrix = ROMTransform.ROMMatrix.Get(matrixIndex, indices, vectors);
				if (vectorIndex != 0xFFFF && vectorIndex < vectors.length) {
					vector = vectors.vectors[vectorIndex];
				}
			}
		}
	}
}
