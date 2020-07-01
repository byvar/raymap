using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Type = OpenSpace.Object.SuperObject.Type;

namespace OpenSpace.PS1 {
	public class SuperObject : OpenSpaceStruct, ILinkedListEntry {
		public uint typeCode;
		public int dataIndex;
		public LinkedList<SuperObject> children;
		public Pointer off_next_brother;
		public Pointer off_prev_brother;
		public Pointer off_parent;
		public Pointer off_matrix1;
		public Pointer off_matrix2;
		public short short_28;
		public short short_2A;
		public short short_2C;
		public short short_2E;
		public short short_30;
		public short short_32;
		public short short_34;
		public short short_36;
		public uint uint_38;
		public Pointer off_38;
		public short short_3C;
		public short short_3E;
		public short short_40;
		public short short_42;
		public short short_44;
		public short short_46;
		public short short_48;
		public short short_4A;

		// Parsed
		public Type type;
		public SuperObject parent;
		public PS1Matrix matrix1;
		public PS1Matrix matrix2;

		public Pointer NextEntry => off_next_brother;
		public Pointer PreviousEntry => off_prev_brother;
		public bool isDynamic = false;

		protected override void ReadInternal(Reader reader) {
			//Load.print("SuperObject @ " + Offset);
			typeCode = reader.ReadUInt32();
			dataIndex = reader.ReadInt32();
			children = LinkedList<SuperObject>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
			off_next_brother = Pointer.Read(reader); // 14
			off_prev_brother = Pointer.Read(reader); // 18
			off_parent = Pointer.Read(reader); // 1c
			off_matrix1 = Pointer.Read(reader); //
			if (Settings.s.game != Settings.Game.RRush) {
				off_matrix2 = Pointer.Read(reader);
				short_28 = reader.ReadInt16();
				short_2A = reader.ReadInt16();
				short_2C = reader.ReadInt16();
				short_2E = reader.ReadInt16();
				short_30 = reader.ReadInt16();
				short_32 = reader.ReadInt16();
				short_34 = reader.ReadInt16();
				short_36 = reader.ReadInt16();
				if (isDynamic) {
					uint_38 = reader.ReadUInt32();
				} else {
					off_38 = Pointer.Read(reader);
				}
				short_3C = reader.ReadInt16();
				short_3E = reader.ReadInt16();
				short_40 = reader.ReadInt16();
				short_42 = reader.ReadInt16();
				short_44 = reader.ReadInt16();
				short_46 = reader.ReadInt16();
				short_48 = reader.ReadInt16();
				short_4A = reader.ReadInt16();
			}

			type = GetSOType(typeCode);
			Load.print(typeCode + "|" + type + " - " + Offset + " - " + children.Count + " - " + dataIndex);

			children.ReadEntries(ref reader, (off_child) => {
				SuperObject child = Load.FromOffsetOrRead<SuperObject>(reader, off_child, onPreRead: s => s.isDynamic = isDynamic);
				return child;
			}, LinkedList.Flags.HasHeaderPointers);
			SuperObject parent = Load.FromOffsetOrRead<SuperObject>(reader, off_parent, onPreRead: s => s.isDynamic = isDynamic);
			matrix1 = Load.FromOffsetOrRead<PS1Matrix>(reader, off_matrix1);
			matrix2 = Load.FromOffsetOrRead<PS1Matrix>(reader, off_matrix2);

		}


		public static Type GetSOType(uint typeCode) {
			Type type = Type.Unknown;
			switch (typeCode) {
				case 0x0: type = Type.World; break;
				case 0x4: type = Type.Perso; break;
				case 0x8: type = Type.Sector; break;
				case 0xD: type = Type.IPO; break;
				case 0x15: type = Type.IPO_2; break;
			}
			return type;
		}

		public OpenSpaceStruct Data {
			get {
				LevelHeader h = (Load as R2PS1Loader).levelHeader;
				if (type == Type.IPO) {
					if ((dataIndex >> 1) >= h.geometricObjectsStatic.entries.Length) throw new Exception("IPO SO data index was too high! " + h.geometricObjectsStatic.entries.Length + " - " + dataIndex);
					return h.geometricObjectsStatic.entries[dataIndex >> 1].geo;
				} else if (type == Type.Perso) {
					if (dataIndex >= h.persos.Length) throw new Exception("Perso SO data index was too high! " + h.persos.Length + " - " + dataIndex);
					return h.persos[dataIndex];
				} else if (type == Type.Sector) {
					if (dataIndex >= h.sectors.Length) throw new Exception("Sector SO data index was too high! " + h.sectors.Length + " - " + dataIndex);
					return h.sectors[dataIndex];
				}
				return null;
			}
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject(type + " @ " + Offset);
			if (type != Type.IPO) {
				matrix1?.Apply(gao);
			}
			/*if (.Value != null) {
				if (data.Value is PhysicalObject) {
					PhysicalObjectComponent poc = ((PhysicalObject)data.Value).GetGameObject(gao);
				} else if (data.Value is Sector) {
					SectorComponent sc = ((Sector)data.Value).GetGameObject(gao);
				}
			}*/
			if (type == Type.IPO) {
				PhysicalObjectComponent poc = gao.AddComponent<PhysicalObjectComponent>();
				LevelHeader h = (Load as R2PS1Loader).levelHeader;
				int ind = (dataIndex >> 1);
				if ((dataIndex >> 1) >= h.geometricObjectsStatic.entries.Length) throw new Exception("IPO SO data index was too high! " + h.geometricObjectsStatic.entries.Length + " - " + dataIndex);
				GameObject g = h.geometricObjectsStatic.GetGameObject(dataIndex >> 1, null, out _);
				if (g != null) {
					poc.visual = g;
					g.transform.SetParent(gao.transform);
					g.transform.localPosition = Vector3.zero;
					g.transform.localRotation = Quaternion.identity;
				}
				if (h.ipoCollision != null && h.ipoCollision.Length > dataIndex >> 1) {
					GameObject c = h.ipoCollision[dataIndex >> 1].GetGameObject();
					if (c != null) {
						poc.collide = c;
						c.transform.SetParent(gao.transform);
						c.transform.localPosition = Vector3.zero;
						c.transform.localRotation = Quaternion.identity;
					}
				}
				poc.Init(MapLoader.Loader.controller);
			} else if (type == Type.Perso) {
				LevelHeader h = (Load as R2PS1Loader).levelHeader;
				if (dataIndex >= h.persos.Length) throw new Exception("Perso SO data index was too high! " + h.persos.Length + " - " + dataIndex);
				gao.name = dataIndex + " - " + gao.name;
				PS1PersoBehaviour ps1Perso = h.persos[dataIndex].GetGameObject(gao);
			} else if (type == Type.Sector) {
				LevelHeader h = (Load as R2PS1Loader).levelHeader;
				if (dataIndex >= h.sectors.Length) throw new Exception("Sector SO data index was too high! " + h.sectors.Length + " - " + dataIndex);
				gao.name = dataIndex + " - " + gao.name;
				SectorComponent sect = h.sectors[dataIndex].GetGameObject(gao);
			}
			if (children != null) {
				foreach (SuperObject so in children) {
					if (so != null) {
						GameObject soGao = so.GetGameObject();
						if (soGao != null) {
							soGao.transform.SetParent(gao.transform);
							if (so.type != Type.IPO) {
								so.matrix1?.Apply(soGao);
							} else {
								soGao.transform.localPosition = Vector3.zero;
								soGao.transform.localRotation = Quaternion.identity;
							}
						}
					}
				}
			}
			return gao;
		}
	}
}
