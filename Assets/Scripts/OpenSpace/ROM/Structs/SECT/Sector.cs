using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Sector : ROMStruct {
		// size: 52 or 0x34
		public Reference<SectorSuperObjectArray1> graphicSectors;
		public Reference<SectorSuperObjectArray1Info> neighborsInfo;
		public Reference<SectorSuperObjectArray2> activitySectors;
		public Reference<SectorSuperObjectArray3> collisionSectors;
		public Reference<LightInfoArray> lights;
		public Reference<SectorSuperObjectArray4> sectors4;
		public Reference<SectorSuperObjectArray4Info> sectors4Info;
		public Reference<SectorSuperObjectArray5> sectors5;
		public Reference<SectorSuperObjectArray5Info> sectors5Info;
		public Reference<CompressedVector3Array> boundingVolume;
		public float float14;
		public float float18;
		public Reference<VisualMaterial> background; // 0x1C
		public byte byte1E;
		public byte byte1F;
		public ushort num_neighbors; // 0x20
		public ushort num_sectors2; // 0x22
		public ushort num_sectors3; // 0x24
		public ushort num_lights; // 0x26
		public ushort num_sectors4; // 0x28
		public ushort num_sectors5; // 0x2A, 42
		public byte isSectorVirtual;
		public byte byte2D;
		public ushort word2E;
		public byte sectorPriority;
		public byte byte31;
		public ushort word32;

		protected override void ReadInternal(Reader reader) {
			graphicSectors = new Reference<SectorSuperObjectArray1>(reader);
			neighborsInfo = new Reference<SectorSuperObjectArray1Info>(reader);
			activitySectors = new Reference<SectorSuperObjectArray2>(reader);
			collisionSectors = new Reference<SectorSuperObjectArray3>(reader);
			lights = new Reference<LightInfoArray>(reader);
			sectors4 = new Reference<SectorSuperObjectArray4>(reader);
			sectors4Info = new Reference<SectorSuperObjectArray4Info>(reader);
			sectors5 = new Reference<SectorSuperObjectArray5>(reader);
			sectors5Info = new Reference<SectorSuperObjectArray5Info>(reader);
			boundingVolume = new Reference<CompressedVector3Array>(reader, true, va => va.length = 2);
			float14 = reader.ReadSingle();
			float18 = reader.ReadSingle();
			background = new Reference<VisualMaterial>(reader, true);
			byte1E = reader.ReadByte();
			byte1F = reader.ReadByte();
			num_neighbors = reader.ReadUInt16();
			num_sectors2 = reader.ReadUInt16();
			num_sectors3 = reader.ReadUInt16();
			num_lights = reader.ReadUInt16();
			num_sectors4 = reader.ReadUInt16();
			num_sectors5 = reader.ReadUInt16();
			isSectorVirtual = reader.ReadByte();
			byte2D = reader.ReadByte();
			word2E = reader.ReadUInt16();
			sectorPriority = reader.ReadByte();
			byte31 = reader.ReadByte();
			word32 = reader.ReadUInt16();


			graphicSectors.Resolve(reader, s1 => s1.length = num_neighbors);
			neighborsInfo.Resolve(reader, s1 => s1.length = num_neighbors);
			activitySectors.Resolve(reader, s2 => s2.length = num_sectors2);
			collisionSectors.Resolve(reader, s3 => s3.length = num_sectors3);
			lights.Resolve(reader, li => li.length = num_lights);
			sectors4.Resolve(reader, s4 => s4.length = num_sectors4);
			sectors4Info.Resolve(reader, s4 => s4.length = num_sectors4);
			sectors5.Resolve(reader, s5 => s5.length = num_sectors5);
			sectors5Info.Resolve(reader, s5 => s5.length = num_sectors5);

		}

		public SectorComponent GetGameObject(GameObject gao) {
			gao.name += " - Sector @ " + Offset; // + " - " + isSectorVirtual + " - " + byte2D + " - " + sectorPriority + " - " + byte31 + " - " + byte1E + " - " + byte1F;
			if (FileSystem.mode == FileSystem.Mode.Web) {
				gao.name = "Sector @ " + Offset;
			}
			SectorComponent sc = gao.AddComponent<SectorComponent>();
			sc.sectorROM = this;
			sc.sectorManager = MapLoader.Loader.controller.sectorManager;
			MapLoader.Loader.controller.sectorManager.AddSector(sc);
			//sc.Init();
			return sc;
		}
	}
}
