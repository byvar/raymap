using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OpenSpace.AI;
using OpenSpace.Collide;
using OpenSpace.Object.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object {
    public class SuperObject : ILinkedListEntry, IReferenceable {
        public enum Type {
            Unknown,
            World,
            IPO,
            IPO_2,
            Perso,
            Sector,
			PhysicalObject,
            GeometricObject, // Geometric Object
            GeometricShadowObject, // Instantiated Geometric Object
        }

        public LegacyPointer offset;
        public uint typeCode;
        public Type type;
        public LegacyPointer off_data;
        public LinkedList<SuperObject> children;
        public LegacyPointer off_brother_next;
        public LegacyPointer off_brother_prev;
        public LegacyPointer off_parent;
        [JsonIgnore]
        public SuperObject parent;
        public LegacyPointer off_matrix;
		public LegacyPointer off_staticMatrix;
		public int globalMatrix;
        public Matrix matrix;
		public Matrix staticMatrix;
        public IEngineObject data;
		public SuperObjectDrawFlags drawFlags;
        public SuperObjectFlags flags;
        public BoundingVolume boundingVolume;
        public GeometricObjectCollide boundingVolumeTT;

        [JsonIgnore]
        public ReferenceFields References { get; set; } = new ReferenceFields();

        private bool isGameObjectInitialized = false;
        public GameObject Gao {
            get {
                if (data != null) {
                    return data.Gao;
                } else return null;
            }
        }

        public async UniTask InitGameObject() {
            if (isGameObjectInitialized) return;
            isGameObjectInitialized = true;

            GameObject gao = data?.Gao;

            if(gao != null) {

                //SuperObjectComponent soc = so.Gao.GetComponent<SuperObjectComponent>();
                foreach (SuperObject ch in children) {
                    if (ch != null) await ch.InitGameObject();
                }
                string previousLoadingState = MapLoader.Loader.loadingState;
                MapLoader.Loader.loadingState = previousLoadingState + "\nInitializing SuperObject: " + gao.name;
                await MapLoader.WaitIfNecessary();
                //await MapLoader.WaitFrame();

                Vector3 pos = Vector3.zero;
                Vector3 scale = Vector3.one;
                Quaternion rot = Quaternion.identity;
                if (matrix != null) {
                    pos = matrix.GetPosition(convertAxes: true);
                    rot = matrix.GetRotation(convertAxes: true);
                    scale = matrix.GetScale(convertAxes: true);
                }

                if (parent != null && parent.Gao != null) Gao.transform.parent = parent.Gao.transform;
                if (pos.IsValid()) Gao.transform.localPosition = pos;
                if (rot.IsValid()) Gao.transform.localRotation = rot;
                if (scale.IsValid()) Gao.transform.localScale = scale;
                if (boundingVolumeTT != null) {
                    boundingVolumeTT.Gao.transform.SetParent(Gao.transform);
                    boundingVolumeTT.Gao.transform.localPosition = Vector3.zero;
                    boundingVolumeTT.Gao.transform.localRotation = Quaternion.identity;
                    boundingVolumeTT.Gao.transform.localScale = Vector3.one;
                }

                SuperObjectComponent soc = Gao.AddComponent<SuperObjectComponent>();
                Gao.layer = LayerMask.NameToLayer("SuperObject");
                soc.so = this;
                MapLoader.Loader.controller.superObjects.Add(soc);

                if (boundingVolume != null) {
                    if (boundingVolume.type == BoundingVolume.Type.Box) {
                        BoxCollider collider = Gao.AddComponent<BoxCollider>();

                        collider.center = boundingVolume.Center;
                        collider.center -= Gao.transform.position;
                        collider.size = boundingVolume.Size;
                    } else {
                        SphereCollider collider = Gao.AddComponent<SphereCollider>();

                        collider.center = boundingVolume.Center;
                        collider.radius = boundingVolume.sphereRadius;
                    }
                }
                
                MapLoader.Loader.loadingState = previousLoadingState;
                await MapLoader.WaitIfNecessary();


                //SuperObjectComponent soc = so.Gao.GetComponent<SuperObjectComponent>();
                foreach (SuperObject ch in children) {
                    //if (ch != null) await ch.InitGameObject();
                    if (soc != null && ch != null && ch.Gao != null) {
                        SuperObjectComponent soc_ch = ch.Gao.GetComponent<SuperObjectComponent>();
                        if (soc_ch == null) continue;
                        soc.Children.Add(soc_ch);
                    }
                }
            }
        }

        public LegacyPointer NextEntry {
            get {
                return off_brother_next;
            }
        }

        public LegacyPointer PreviousEntry {
            get {
                return off_brother_prev;
            }
        }

        /// <summary>
        /// Check in which sector this SuperObject resides by recursively checking the parent SPO's until encountering a sector type
        /// </summary>
        public SuperObject ParentSector
        {
            get
            {
                if (this.type == Type.Sector) return this;
                return parent?.ParentSector;
            }
        }

        public SuperObject(LegacyPointer offset) {
            this.offset = offset;
        }

        public static SuperObject Read(Reader reader, LegacyPointer off_so, SuperObject parent = null) {
            MapLoader l = MapLoader.Loader;
            if (IsParsed(off_so)) return null;
            bool isValidNode = true;
            SuperObject so = new SuperObject(off_so);
            l.superObjects.Add(so); // Global list of superobjects (all)
            if (parent != null) {
                so.parent = parent;
            }
            so.typeCode = reader.ReadUInt32(); // 0 - 4
            so.off_data = LegacyPointer.Read(reader); // 4 - 8
            so.children = LinkedList<SuperObject>.ReadHeader(reader, LegacyPointer.Current(reader), LinkedList.Type.Double); // 8 - 14
            so.off_brother_next = LegacyPointer.Read(reader); // 14 - 18
            so.off_brother_prev = LegacyPointer.Read(reader); // 18 - 1C
            so.off_parent = LegacyPointer.Read(reader); // 1C - 20
            so.off_matrix = LegacyPointer.Read(reader); // 0x20->0x24
            so.off_staticMatrix = LegacyPointer.Read(reader); // other matrix
            so.globalMatrix = reader.ReadInt32(); // 0x28 -> 0x2C
			so.drawFlags = SuperObjectDrawFlags.Read(reader);
            so.flags = SuperObjectFlags.Read(reader); // 0x30->0x34
            if (Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.R3) reader.ReadUInt32(); // Visual Bounding Volume?
            LegacyPointer off_boundingVolume = LegacyPointer.Read(reader);
            //l.print("SuperObject T" + so.typeCode + ": " + off_so + " - " + so.off_matrix);

			if(Legacy_Settings.s.game == Legacy_Settings.Game.R2Beta && so.typeCode == 11)
				return so;

            LegacyPointer.DoAt(ref reader, so.off_matrix, () => {
                so.matrix = Matrix.Read(reader, so.off_matrix);
            });
			/*Pointer.DoAt(ref reader, so.off_matrix2, () => {
				so.matrix2 = Matrix.Read(reader, so.off_matrix2);
				if (so.matrix == null) {
					pos = so.matrix2.GetPosition(convertAxes: true);
					rot = so.matrix2.GetRotation(convertAxes: true);
					scale = so.matrix2.GetScale(convertAxes: true);
				}
			});*/
			so.type = GetSOType(so.typeCode);
            switch (so.type) {
                case Type.IPO:
                    LegacyPointer.DoAt(ref reader, so.off_data, () => {
                        so.data = IPO.Read(reader, so.off_data, so);
                    });
                    break;
                case Type.IPO_2:
                    LegacyPointer.DoAt(ref reader, so.off_data, () => {
                        l.print("IPO with code 0x40 at offset " + so.off_data);
                        so.data = IPO.Read(reader, so.off_data, so);
                    });
                    break;
                case Type.PhysicalObject:
                    if (!Legacy_Settings.s.loadFromMemory) {
                        LegacyPointer.DoAt(ref reader, so.off_data, () => {
                            so.data = PhysicalObject.Read(reader, so.off_data, so);
                        });
                    }
                    break;
                case Type.Perso:
                    LegacyPointer.DoAt(ref reader, so.off_data, () => {
                        so.data = Perso.Read(reader, so.off_data, so);
                    });
                    break;
                case Type.World:
                    so.data = World.New(so);
                    //print("parsing world superobject with " + num_children + " children");
                    break;
                case Type.Sector:
                    LegacyPointer.DoAt(ref reader, so.off_data, () => {
                        so.data = Sector.Read(reader, so.off_data, so);
                    });
                    break;
                case Type.GeometricObject:
                    LegacyPointer.DoAt(ref reader, so.off_data, () => {
                        so.data = Visual.GeometricObject.Read(reader, so.off_data);
                    });
                    break;
                case Type.GeometricShadowObject:
                    LegacyPointer.DoAt(ref reader, so.off_data, () => {
                        so.data = Visual.GeometricShadowObject.Read(reader, so.off_data, so);
                    });
                    break;
                default:
                    l.print("Unknown SO type " + so.typeCode + " at " + so.offset + " - " + so.off_data);
                    //isValidNode = false;
                    break;
            }

            LegacyPointer.DoAt(ref reader, off_boundingVolume, () => {
				//l.print(so.type + " - " + so.offset + " - " + off_boundingVolume);
                if (Legacy_Settings.s.engineVersion <= Legacy_Settings.EngineVersion.Montreal) {
                    so.boundingVolumeTT = GeometricObjectCollide.Read(reader, off_boundingVolume, isBoundingVolume: true);
                } else {
                    so.boundingVolume = BoundingVolume.Read(reader, off_boundingVolume, so.flags.HasFlag(SuperObjectFlags.Flags.BoundingBoxInsteadOfSphere) ?
                        BoundingVolume.Type.Box : BoundingVolume.Type.Sphere);
                }
            });

            if (isValidNode) {
                so.children.ReadEntries(ref reader, (off_child) => {
                    SuperObject child = SuperObject.FromOffsetOrRead(off_child, reader, parent: so);
                    child.parent = so;
                    return child;
                }, LinkedList.Flags.HasHeaderPointers);
            }
            return so;
        }

        public static SuperObject FromOffset(LegacyPointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.superObjects.FirstOrDefault(so => so.offset == offset);
        }

        public static SuperObject FromOffsetOrRead(LegacyPointer offset, Reader reader, SuperObject parent = null) {
            if (offset == null) return null;
            SuperObject so = FromOffset(offset);
            if (so == null) {
                LegacyPointer.DoAt(ref reader, offset, () => {
                    so = SuperObject.Read(reader, offset, parent: parent);
                });
            } else {
                if (parent != null && so.parent != parent) so.parent = parent;
            }
            return so;
        }

        public static bool IsParsed(LegacyPointer offset) {
            return FromOffset(offset) != null;
        }

        public static Type GetSOType(uint typeCode) {
            Type type = Type.Unknown;
            if (Legacy_Settings.s.engineVersion > Legacy_Settings.EngineVersion.Montreal) {
                switch (typeCode) {
                    case 0x1: type = Type.World; break;
                    case 0x2: type = Type.Perso; break;
                    case 0x4: type = Type.Sector; break;
					case 0x8: type = Type.PhysicalObject; break;
                    case 0x20: type = Type.IPO; break;
                    case 0x40: type = Type.IPO_2; break;
                    case 0x400: type = Type.GeometricObject; break;
                    case 0x80000: type = Type.GeometricShadowObject; break;
                }
            } else {
                switch (typeCode) {
                    case 0x0: type = Type.World; break;
                    case 0x4: type = Type.Perso; break;
                    case 0x8: type = Type.Sector; break;
					case 10: type = Type.GeometricObject; break;
					case 11: type = Type.PhysicalObject; break;
                    case 0xD: type = Type.IPO; break;
                    case 0x15: type = Type.IPO_2; break;
                }
            }
            return type;
        }
    }
}
