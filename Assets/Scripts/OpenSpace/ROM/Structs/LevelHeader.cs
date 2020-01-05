using OpenSpace.Loader;

namespace OpenSpace.ROM {
	// Size: 3DS: 60
	// Size: RRR: 60
	// Size:  DS: 56
	public class LevelHeader : ROMStruct {
		public Reference<HierarchyRoot> hierarchyRoot; // 45 has size 12
		public ushort ref_46; // 46 = array of ref_45s
		public Reference<SuperObjectDynamicArray> spawnablePersos;
		public ushort ref_144;
		public ushort ref_143;
		public ushort unk6;
		public ushort unk7_3dsOnly;
		public ushort length_46;
		public ushort num_spawnablepersos;
		public ushort unk10_3dsOnly;
		public ushort unk11;
		public ushort unk12;
		public ushort unk13;
		public ushort unk14;
		public ushort unk15;
		public ushort unk16;
		public ushort unk17;
		public ushort unk18;
		public ushort unk19;
		public ushort unk20;
		public ushort unk21;
		public ushort unk22;
		public ushort unk23;
		public ushort unk24;
		public ushort unk25;
		public Reference<Vector3Array> vectors;
		public Reference<Short3Array> indices;
		public ushort len_vectors;
		public ushort len_indices;
		public ushort unk30;
		public Reference<VisualMaterial> backgroundUpLeft;
		public Reference<VisualMaterial> backgroundUpRight;
		public Reference<VisualMaterial> backgroundDownLeft;
		public Reference<VisualMaterial> backgroundDownRight;

		protected override void ReadInternal(Reader reader) {
			MapLoader.Loader.print("Level Header: " + Pointer.Current(reader));
			hierarchyRoot = new Reference<HierarchyRoot>(reader);
			ref_46 = reader.ReadUInt16();
			spawnablePersos = new Reference<SuperObjectDynamicArray>(reader);
			ref_144 = reader.ReadUInt16();
			ref_143 = reader.ReadUInt16();
			unk6 = reader.ReadUInt16();
			if (Settings.s.platform == Settings.Platform._3DS || Settings.s.game == Settings.Game.RRR) {
				unk7_3dsOnly = reader.ReadUInt16();
			}
			length_46 = reader.ReadUInt16();
			num_spawnablepersos = reader.ReadUInt16();
			if (Settings.s.platform == Settings.Platform._3DS || Settings.s.game == Settings.Game.RRR) {
				unk10_3dsOnly = reader.ReadUInt16();
			}
			unk11 = reader.ReadUInt16();
			unk12 = reader.ReadUInt16();
			unk13 = reader.ReadUInt16();
			unk14 = reader.ReadUInt16();
			unk15 = reader.ReadUInt16();
			unk16 = reader.ReadUInt16();
			unk17 = reader.ReadUInt16();
			unk18 = reader.ReadUInt16();
			unk19 = reader.ReadUInt16();
			unk20 = reader.ReadUInt16();
			unk21 = reader.ReadUInt16();
			unk22 = reader.ReadUInt16();
			unk23 = reader.ReadUInt16();
			unk24 = reader.ReadUInt16();
			unk25 = reader.ReadUInt16();
			vectors = new Reference<Vector3Array>(reader);
			indices = new Reference<Short3Array>(reader);
			len_vectors = reader.ReadUInt16();
			len_indices = reader.ReadUInt16();
			unk30 = reader.ReadUInt16(); // 1 is divided by this one
			if (Settings.s.game == Settings.Game.DD) {
				backgroundUpLeft = new Reference<VisualMaterial>(reader, true);
				backgroundUpRight = new Reference<VisualMaterial>(reader, true);
				backgroundDownLeft = new Reference<VisualMaterial>(reader, true);
				backgroundDownRight = new Reference<VisualMaterial>(reader, true);
			}

			vectors.Resolve(reader, v => { v.length = len_vectors; });
			indices.Resolve(reader, i => { i.length = len_indices; });
			hierarchyRoot.Resolve(reader);
			spawnablePersos.Resolve(reader, a => a.length = num_spawnablepersos);
			//Loader.print("Spawnable persos: " + num_spawnablepersos + " - " + spawnablePersos.Value.superObjects[0].Value.Offset);
		}
    }
}
