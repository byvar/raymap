using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Type = OpenSpace.Object.SuperObject.Type;

namespace OpenSpace.PS1 {
	public class ObjectsTable : OpenSpaceStruct {
		public uint unk0;
		public uint unk1;
		public Entry[] entries;
		public uint? length;

		protected override void ReadInternal(Reader reader) {
			unk0 = reader.ReadUInt32();
			unk1 = reader.ReadUInt32();
			if (length.HasValue) {
				entries = new Entry[length.Value];
				for (int i = 0; i < entries.Length; i++) {
					entries[i] = new Entry();
					entries[i].Read(reader);
				}
			} else {
				List<Entry> entries = new List<Entry>();
				Entry entry = new Entry();
				entry.Read(reader);
				while (entry.off_geo != null) {
					entries.Add(entry);
					entry = new Entry();
					entry.Read(reader);
				}
				length = (uint)entries.Count;
				this.entries = entries.ToArray();
			}
		}

		public void ReadExtra(Reader reader, uint count) {
			Array.Resize(ref entries, (int)(length + count));
			LegacyPointer.DoAt(ref reader, Offset + 8 + (8 * length.Value), () => {
				//Load.print(Pointer.Current(reader) + " - " + count);
				for (int i = (int)length; i < entries.Length; i++) {
					entries[i] = new Entry();
					entries[i].Read(reader);
				}
			});
			length += count;
		}

		public class Entry {
			public LegacyPointer offset;

			public LegacyPointer off_0; // object of 0x50, 5 rows of 0x10
			public LegacyPointer off_geo;
			public GeometricObject geo;

			public void Read(Reader reader) {
				offset = LegacyPointer.Current(reader);

				off_0 = LegacyPointer.Read(reader);
				off_geo = LegacyPointer.Read(reader);
				geo = Load.FromOffsetOrRead<GeometricObject>(reader, off_geo);
			}
			public GameObject GetGameObject(PhysicalObjectCollisionMapping[] collision, out GameObject[] bones, Entry morphEntry = null, float morphProgress = 0f) {
				bones = null;
				GameObject gao = geo?.GetGameObject(out bones, morphObject: morphEntry?.geo, morphProgress: morphProgress);
				if (gao != null) {
					GameObject wrapper = new GameObject(gao.name + " - Wrapper");
					if (morphProgress > 0 || morphEntry != null) {
						gao.name += " - Morph Progress: " + morphProgress;
					}
					gao.transform.SetParent(wrapper.transform);
					gao.transform.localPosition = Vector3.zero;
					gao.transform.localRotation = Quaternion.identity;
					gao.transform.localScale = Vector3.one;
					PhysicalObjectComponent poc = wrapper.AddComponent<PhysicalObjectComponent>();
					poc.visual = gao;
					if (collision != null) {
						PhysicalObjectCollisionMapping cm = collision.FirstOrDefault(c => c.off_poListEntry == offset);
						if (cm != null && cm.geo_collide != null) {
							GameObject cgao = cm.geo_collide.GetGameObject();
							cgao.transform.SetParent(wrapper.transform);
							cgao.transform.localPosition = Vector3.zero;
							cgao.transform.localRotation = Quaternion.identity;
							cgao.transform.localScale = Vector3.one;
							poc.collide = cgao;
						}
					}
					poc.Init(MapLoader.Loader.controller);
					return wrapper;
				} else return gao;
			}
		}

		public GameObject GetGameObject(int i, PhysicalObjectCollisionMapping[] collision, out GameObject[] bones, int? morphI = null, float morphProgress = 0f) {
			bones = null;
			if (i < 0 || i >= length) return null;
			return entries[i]?.GetGameObject(
				collision,
				out bones,
				morphEntry: ((morphI != null && morphI.Value >= 0 && morphI.Value < length) ? entries[morphI.Value] : null),
				morphProgress: morphProgress);
		}
	}
}
