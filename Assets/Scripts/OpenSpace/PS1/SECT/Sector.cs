using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class Sector : OpenSpaceStruct {
		public Pointer off_persos;
		public Pointer off_graphicSectors;
		public Pointer off_collisionSectors;
		public Pointer off_activitySectors;
		public Pointer off_10;
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

		// VIP
		public ushort vip_ushort_08;
		public ushort vip_ushort_0A;
		public uint vip_uint_0C;
		public Pointer off_so;

		// Parsed
		public uint[] persos;
		public NeighborSector[] graphicSectors;
		public NeighborSector[] collisionSectors;
		public NeighborSector[] activitySectors;

		protected override void ReadInternal(Reader reader) {
			Load.print("Sector @ " + Offset);
			off_persos = Pointer.Read(reader);
			off_graphicSectors = Pointer.Read(reader);
			if (CPA_Settings.s.game != CPA_Settings.Game.R2 && CPA_Settings.s.game != CPA_Settings.Game.RRush) {
				vip_ushort_08 = reader.ReadUInt16();
				vip_ushort_0A = reader.ReadUInt16();
				vip_uint_0C = reader.ReadUInt32();
			}
			off_collisionSectors = Pointer.Read(reader);
			off_activitySectors = Pointer.Read(reader);
			off_10 = Pointer.Read(reader); // Sound sectors?
			off_ipos = Pointer.Read(reader);
			if (CPA_Settings.s.game != CPA_Settings.Game.R2 && CPA_Settings.s.game != CPA_Settings.Game.RRush) {
				int_50 = reader.ReadInt32();
			} else {
				off_18 = Pointer.Read(reader);
			}
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
			if (CPA_Settings.s.game != CPA_Settings.Game.JungleBook && CPA_Settings.s.game != CPA_Settings.Game.VIP) {
				short_48 = reader.ReadInt16();
				short_4A = reader.ReadInt16();
				int_4C = reader.ReadInt32();
				if (CPA_Settings.s.game == CPA_Settings.Game.R2
					|| CPA_Settings.s.game == CPA_Settings.Game.RRush) {
					int_50 = reader.ReadInt32();
				}
			} else {
				int_4C = reader.ReadInt32();
				int_50 = reader.ReadInt32();
				off_so = Pointer.Read(reader);
			}

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
			Pointer.DoAt(ref reader, off_graphicSectors, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					graphicSectors = Load.ReadArray<NeighborSector>(length, reader, off_array);
				}
			});
			Pointer.DoAt(ref reader, off_collisionSectors, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					collisionSectors = Load.ReadArray<NeighborSector>(length, reader, off_array);
				}
			});
			Pointer.DoAt(ref reader, off_activitySectors, () => {
				uint length = reader.ReadUInt32();
				if (length > 0) {
					Pointer off_array = Pointer.Read(reader);
					activitySectors = Load.ReadArray<NeighborSector>(length, reader, off_array);
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
				return new Vector3(minX, minZ, minY) / R2PS1Loader.CoordinateFactor;
			} else {
				return new Vector3(minX, minY, minZ) / R2PS1Loader.CoordinateFactor;
			}
		}

		public Vector3 GetMaxPoint(bool convertAxes = true) {
			if (convertAxes) {
				return new Vector3(maxX, maxZ, maxY) / R2PS1Loader.CoordinateFactor;
			} else {
				return new Vector3(maxX, maxY, maxZ) / R2PS1Loader.CoordinateFactor;
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
