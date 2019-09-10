using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class LevelList : ROMStruct {
		public Level[] levels;
		public ushort num_levels;

		protected override void ReadInternal(Reader reader) {
			levels = new Level[num_levels];
			for (int i = 0; i < num_levels; i++) {
				levels[i] = new Level(reader);
			}
		}

		public struct Level {
			public ushort index;
			public string name;

			public Level(Reader reader) {
				index = reader.ReadUInt16();
				name = reader.ReadString(62);
			}
		}
    }
}
