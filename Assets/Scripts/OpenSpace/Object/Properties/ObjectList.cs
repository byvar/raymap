using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using OpenSpace.Exporter;
using Newtonsoft.Json.Linq;
using OpenSpace.Visual;

namespace OpenSpace.Object.Properties {
    public class ObjectList : ILinkedListEntry, IList<ObjectListEntry> {
        public Pointer offset;
        public Pointer off_objList_next = null;
        public Pointer off_objList_prev = null;
        public Pointer off_objList_hdr = null; // at this offset, start and end pointers appear again
		
        public Pointer off_objList_start;
        public Pointer off_objList_2;
        public ushort num_entries;
        public string unknownFamilyName;

        public ObjectListEntry[] entries;
        [JsonIgnore] public List<Family> containingFamilies = new List<Family>();

        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("Object List @ " + offset.ToString());
                }
                return gao;
            }
        }

        public Pointer NextEntry {
            get { return off_objList_next; }
        }

        public Pointer PreviousEntry {
            get { return off_objList_prev; }
        }

        public int Count {
            get {
                return ((IList<ObjectListEntry>)entries).Count;
            }
        }

        public bool IsReadOnly {
            get {
                return ((IList<ObjectListEntry>)entries).IsReadOnly;
            }
        }

        public ObjectListEntry this[int index] {
            get {
                return ((IList<ObjectListEntry>)entries)[index];
            }

            set {
                ((IList<ObjectListEntry>)entries)[index] = value;
            }
        }

        public ObjectList(Pointer offset) {
            this.offset = offset;
        }

        public static ObjectList Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            ObjectList ol = new ObjectList(offset);
            //l.print("ObjectList: " + Pointer.Current(reader));
            if(Settings.s.linkedListType != LinkedList.Type.Minimize) ol.off_objList_next = Pointer.Read(reader);
			if (Settings.s.hasLinkedListHeaderPointers) {
                ol.off_objList_prev = Pointer.Read(reader);
                ol.off_objList_hdr = Pointer.Read(reader);
            }
            ol.off_objList_start = Pointer.Read(reader);
            ol.off_objList_2 = Pointer.Read(reader); // is this a copy of the list or something?
            ol.num_entries = reader.ReadUInt16();
            reader.ReadUInt16();

            if (Settings.s.linkedListType == LinkedList.Type.Minimize) ol.off_objList_next = Pointer.Current(reader);

			//l.print("ObjectList " + offset + " - " + ol.num_entries);

            if (ol.off_objList_start != null) {
                Pointer.Goto(ref reader, ol.off_objList_start);
                ol.entries = new ObjectListEntry[ol.num_entries];
                for (uint i = 0; i < ol.num_entries; i++) {
                    // each entry is 0x14
                    ol.entries[i] = new ObjectListEntry();
					if (Settings.s.game == Settings.Game.LargoWinch) {
						ol.entries[i].off_po = Pointer.Read(reader);
					} else {
						ol.entries[i].off_scale = Pointer.Read(reader);
						ol.entries[i].off_po = Pointer.Read(reader);
						ol.entries[i].thirdvalue = reader.ReadUInt32();
						ol.entries[i].unk0 = reader.ReadUInt16();
						ol.entries[i].unk1 = reader.ReadUInt16();
						if (Settings.s.platform != Settings.Platform.DC) {
							ol.entries[i].lastvalue = reader.ReadUInt32();
						}
					}
                    // TODO: Figure out what this points to: if(off_po != null && lastvalue == 0) l.print(off_po);
                    if (ol.entries[i].lastvalue != 0 || ol.entries[i].thirdvalue != 0 || Settings.s.engineVersion == Settings.EngineVersion.TT) {
                        ol.entries[i].po = null;
                        ol.entries[i].scale = null;
                        Pointer.DoAt(ref reader, ol.entries[i].off_scale, () => {
                            float x = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            ol.entries[i].scale = new Vector3(x, y, z);
                        });
                        ol.ReadPO(reader, (int)i);
                    }
                }
            }

            //if (l.mode == MapLoader.Mode.RaymanArenaGC) ol.off_objList_next = Pointer.Current(reader);

            /*if (l.mode == MapLoader.Mode.Rayman3GC) {
                Pointer off_list_hdr_next = Pointer.Read(reader);
                Pointer off_list_hdr_prev = Pointer.Read(reader);
                Pointer off_list_hdr = Pointer.Read(reader);
                //if (off_list_hdr != null) Pointer.Goto(ref reader, off_list_hdr);
            } else if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.RaymanArenaPC) {
                reader.ReadUInt32(); // 0
            } else if (l.mode == MapLoader.Mode.Rayman2PC) {
                Pointer off_list_hdr = Pointer.Read(reader);
                //if (off_list_hdr != null) Pointer.Goto(ref reader, off_list_hdr);
            }
            if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.Rayman3GC) {
                Pointer off_list_hdr_1 = Pointer.Read(reader); // copy of off_subblocklist?
                Pointer off_list_hdr_2 = Pointer.Read(reader); // same?
                reader.ReadUInt32(); // 1?
            }*/
            return ol;
		}

		public void ReadPO(Reader reader, int i) {
			if (entries[i].po != null) return;
			Pointer.DoAt(ref reader, entries[i].off_po, () => {
				entries[i].po = PhysicalObject.Read(reader, entries[i].off_po);
				if (entries[i].po != null && entries[i].scale.HasValue) {
					entries[i].po.scaleMultiplier = entries[i].scale.Value;
				}
				if (entries[i].po != null && entries[i].po.Gao != null) {
					entries[i].po.Gao.transform.SetParent(Gao.transform);
				}
			});
		}

		public static ObjectList FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.objectLists.FirstOrDefault(f => f.offset == offset);
        }

        public static ObjectList FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            ObjectList ol = FromOffset(offset);
            if (ol == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    ol = ObjectList.Read(reader, offset);
                    MapLoader.Loader.objectLists.Add(ol);
                });
            }
            return ol;
        }

        /* Some object lists can only be found in scripts or dsgvars.
        These don't have a reference to the families they're part of, but if any have been loaded, we can try to find their siblings by their pointers.
        */
        public ObjectList FindBrother() {
            List<ObjectList> ols = MapLoader.Loader.objectLists;
            foreach (ObjectList ol in ols) {
                if (ol == null || ol.offset == offset) continue;
                if (off_objList_hdr != null && ol.off_objList_hdr == off_objList_hdr) return ol;
                if (off_objList_next != null
                    && (ol.offset == off_objList_next
                    || ol.off_objList_prev == off_objList_next
                    || ol.off_objList_next == off_objList_next)) return ol;
                if (off_objList_prev != null
                    && (ol.offset == off_objList_prev
                    || ol.off_objList_prev == off_objList_prev
                    || ol.off_objList_next == off_objList_prev)) return ol;
                if (ol.off_objList_prev == offset
                    || ol.off_objList_next == offset) return ol;
            }
            return null;
        }

        public void AddToFamilyLists(Perso perso) {
            if (containingFamilies.Count > 0) return;
            ObjectList brother = FindBrother();
            if (brother != null && brother.containingFamilies.Count > 0) {
                Family f = brother.containingFamilies.First();
				/*foreach (ObjectListEntry ole in this) {
                    if (ole.po != null && ole.po.Gao != null) {
                        ole.po.Gao.transform.SetParent(f.Gao.transform);
                    }
                }*/
                foreach (Family fam in brother.containingFamilies) {
                    fam.AddNewPhysicalList(this);
                }
            } else {

                if (Settings.s.linkUncategorizedObjectsToScriptFamily) {
                    perso.p3dData.family.AddNewPhysicalList(this);
                } else {
                    MapLoader.Loader.AddUncategorizedObjectList(this);
                    Gao.name = "[" + unknownFamilyName + "] Uncategorized Object List @ " + offset.ToString();
                }
            }
        }

        public int IndexOf(ObjectListEntry item) {
            return ((IList<ObjectListEntry>)entries).IndexOf(item);
        }

        public void Insert(int index, ObjectListEntry item) {
            ((IList<ObjectListEntry>)entries).Insert(index, item);
        }

        public void RemoveAt(int index) {
            ((IList<ObjectListEntry>)entries).RemoveAt(index);
        }

        public void Add(ObjectListEntry item) {
            ((IList<ObjectListEntry>)entries).Add(item);
        }

        public void Clear() {
            ((IList<ObjectListEntry>)entries).Clear();
        }

        public bool Contains(ObjectListEntry item) {
            return ((IList<ObjectListEntry>)entries).Contains(item);
        }

        public void CopyTo(ObjectListEntry[] array, int arrayIndex) {
            ((IList<ObjectListEntry>)entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(ObjectListEntry item) {
            return ((IList<ObjectListEntry>)entries).Remove(item);
        }

        public IEnumerator<ObjectListEntry> GetEnumerator() {
            return ((IList<ObjectListEntry>)entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IList<ObjectListEntry>)entries).GetEnumerator();
        }

        public override string ToString() {
            if (containingFamilies.Count > 0) {
                return "Object List @ " + offset.ToString();
            } else {
                return "[" + unknownFamilyName + "] Uncategorized Object List @ " + offset.ToString();
            }
        }

        public class ObjectListReferenceJsonConverter : JsonConverter {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ObjectList);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                ObjectList objectList = (ObjectList)value;
                string hash = HashUtils.MD5Hash(objectList.ToJSON());

                var jt = JToken.FromObject(new ObjectListReference() { Hash = hash });
                jt.WriteTo(writer);
            }
        }

        public string ToJSON()
        {
            JsonSerializerSettings settings = MapExporter.JsonExportSettings;

            settings.Converters.Add(new UnityConverter());
            settings.Converters.Add(new GameMaterial.GameMaterialReferenceJsonConverter());
            settings.Converters.Add(new VisualMaterial.VisualMaterialReferenceJsonConverter());
            
            return JsonConvert.SerializeObject(this, settings);
        }

        public class ObjectListReference {
            public string Hash;
        }
    }
}
