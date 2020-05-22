using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class Sector : OpenSpaceStruct {
		public Pointer off_persos;
		public Pointer off_neighbors;
		public Pointer off_sectors_unk1;
		public Pointer off_sectors_unk2;
		public Pointer off_sectors_unk3;
		public Pointer off_ipos;
		public Pointer off_18;
		public int minX;
		public int minY;
		public int minZ;
		public int int_28;
		public int maxX;
		public int maxY;
		public int maxZ;
		public int int_38;
		public int int_3C;
		public short short_40;
		public short short_42;
		public short short_44;
		public short short_46;
		public short short_48;
		public short short_4A;
		public int int_4C;
		public int int_50;

		// Parsed
		public uint[] persos;
		public NeighborSector[] neighbors;
		public NeighborSector[] sectors_unk1;
		public NeighborSector[] sectors_unk2;
		public NeighborSector[] sectors_unk3;

		protected override void ReadInternal(Reader reader) {
			Load.print("Sector @ " + Offset);
			off_persos = Pointer.Read(reader);
			off_neighbors = Pointer.Read(reader);
			off_sectors_unk1 = Pointer.Read(reader);
			off_sectors_unk2 = Pointer.Read(reader);
			off_sectors_unk3 = Pointer.Read(reader);
			off_ipos = Pointer.Read(reader);
			off_18 = Pointer.Read(reader);
			minX = reader.ReadInt32();
			minY = reader.ReadInt32();
			minZ = reader.ReadInt32();
			int_28 = reader.ReadInt32();
			maxX = reader.ReadInt32();
			maxY = reader.ReadInt32();
			maxZ = reader.ReadInt32();
			int_38 = reader.ReadInt32();
			int_3C = reader.ReadInt32();
			short_40 = reader.ReadInt16();
			short_42 = reader.ReadInt16();
			short_44 = reader.ReadInt16();
			short_46 = reader.ReadInt16();
			short_48 = reader.ReadInt16();
			short_4A = reader.ReadInt16();
			int_4C = reader.ReadInt32();
			int_50 = reader.ReadInt32();

			Pointer.DoAt(ref reader, off_persos, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					Load.print("Persos: " + length);
				}
			});
			Pointer.DoAt(ref reader, off_ipos, () => {
				uint length = reader.ReadUInt32();
				if (length > 1) {
					Pointer off_array = Pointer.Read(reader);
					Load.print("IPOs: " + length);
				}
			});
			Pointer.DoAt(ref reader, off_neighbors, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					neighbors = Load.ReadArray<NeighborSector>(length, reader, off_array);
				}
			});
			Pointer.DoAt(ref reader, off_sectors_unk1, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					sectors_unk1 = Load.ReadArray<NeighborSector>(length, reader, off_array);
				}
			});
			Pointer.DoAt(ref reader, off_sectors_unk2, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					sectors_unk2 = Load.ReadArray<NeighborSector>(length, reader, off_array);
				}
			});
			Pointer.DoAt(ref reader, off_sectors_unk3, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					sectors_unk3 = Load.ReadArray<NeighborSector>(length, reader, off_array);
				}
			});
		}

		public SectorComponent GetGameObject(GameObject gao) {
			gao.name += " - Sector @ " + Offset; // + " - " + isSectorVirtual + " - " + byte2D + " - " + sectorPriority + " - " + byte31 + " - " + byte1E + " - " + byte1F;
			SectorComponent sc = gao.AddComponent<SectorComponent>();
			sc.sectorPS1 = this;
			sc.sectorManager = MapLoader.Loader.controller.sectorManager;
			MapLoader.Loader.controller.sectorManager.AddSector(sc);
			//sc.Init();
			return sc;
		}

		public Vector3 GetMinPoint(bool convertAxes = true) {
			if (convertAxes) {
				return new Vector3(minX / 256f, minZ / 256f, minY / 256f);
			} else {
				return new Vector3(minX / 256f, minY / 256f, minZ / 256f);
			}
		}

		public Vector3 GetMaxPoint(bool convertAxes = true) {
			if (convertAxes) {
				return new Vector3(maxX / 256f, maxZ / 256f, maxY / 256f);
			} else {
				return new Vector3(maxX / 256f, maxY / 256f, maxZ / 256f);
			}
		}

		public OpenSpace.Collide.BoundingVolume BoundingVolume {
			get {
				Vector3 min = GetMinPoint();
				Vector3 max = GetMaxPoint();
				return new OpenSpace.Collide.BoundingVolume(null) {
					type = OpenSpace.Collide.BoundingVolume.Type.Box,
					boxMin = min,
					boxMax = max,
					//boxCenter = Vector3.Lerp(vectors[0], vectors[1], 0.5f),
					boxSize = max - min,
					boxCenter = min + (max - min) * 0.5f
				};
			}
		}
	}
}
