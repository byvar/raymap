using Newtonsoft.Json;
using OpenSpace.Collide;
using OpenSpace.Visual;
using OpenSpace.Visual.Deform;
using OpenSpace.Visual.ISI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSpace.Object.Properties;
using UnityEngine;

namespace OpenSpace.Object {
    public class PhysicalObject : IEquatable<PhysicalObject>, IEngineObject {
        public LegacyPointer offset;
        public LegacyPointer off_visualSet;
        public LegacyPointer off_collideSet;
        public LegacyPointer off_visualBoundingVolume;
        public LegacyPointer off_collideBoundingVolume;
        public VisualSetLOD[] visualSet;
        public ushort visualSetType = 0;
        public GeometricObjectCollide collideMesh;
        public Vector3? scaleMultiplier = null;

        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) InitGameObject();
                return gao;
            }
        }
        public DeformSet Bones {
            get {
                for (int i = 0; i < visualSet.Length; i++) {
                    if (visualSet[i].obj != null && visualSet[i].obj is GeometricObject && ((GeometricObject)visualSet[i].obj).bones != null) {
                        return ((GeometricObject)visualSet[i].obj).bones;
                    }
                }
                return null;
            }
		}

        private void InitGameObject() {
            gao = new GameObject("[PO]");
            for (int i = 0; i < visualSet.Length; i++) {
                if (visualSet[i].obj == null) continue;

                switch (visualSet[i].obj) {
                    case GeometricObject m:
                        if (m.name != "Mesh") Gao.name = "[PO] " + m.name;
                        break;
                    case PatchGeometricObject mod:
                        if (mod.mesh != null && mod.mesh.name != "Mesh") {
                            Gao.name = "[PO] " + mod.mesh.name;
                        }
                        Gao.name += " - Patch";
                        break;
                }
                // Initialize children
                if (visualSet[i].obj.Gao != null) {
                    visualSet[i].obj.Gao.transform.parent = Gao.transform;
                }
            }

            if (visualSet.Length > 1) { // = number of LOD
                LODComponent lod = Gao.AddComponent<LODComponent>();
                lod.visualSet = visualSet;
                lod.gameObjects = visualSet.Select(v => v.obj.Gao).ToArray();
                /*float bestLOD = po.visualSet.Min(v => v.LODdistance);
                foreach (VisualSetLOD lod in po.visualSet) {
                    if (lod.obj.Gao != null && lod.LODdistance != bestLOD) lod.obj.Gao.SetActive(false);
                }*/
            }
            if (collideMesh != null && collideMesh.Gao != null) {
                collideMesh.Gao.transform.parent = Gao.transform;
            }
        }

		private SuperObject superObject;
		public SuperObject SuperObject {
			get { return superObject; }
		}

		public PhysicalObject(LegacyPointer offset, SuperObject so = null) {
            this.offset = offset;
			this.superObject = so;
            visualSet = new VisualSetLOD[0];
        }
        public override bool Equals(System.Object obj) {
            return obj is PhysicalObject && this == (PhysicalObject)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode();
        }

        public bool Equals(PhysicalObject other) {
            return this == (PhysicalObject)other;
        }

        public static bool operator ==(PhysicalObject x, PhysicalObject y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset;
        }
        public static bool operator !=(PhysicalObject x, PhysicalObject y) {
            return !(x == y);
        }

        public static PhysicalObject Read(Reader reader, LegacyPointer offset, SuperObject so = null, Radiosity radiosity = null) {
            PhysicalObject po = new PhysicalObject(offset, so);
			//MapLoader.Loader.print("PO @ " + offset);
			// Header
			po.off_visualSet = LegacyPointer.Read(reader);
            po.off_collideSet = LegacyPointer.Read(reader);
            po.off_visualBoundingVolume = LegacyPointer.Read(reader);
            if (Legacy_Settings.s.engineVersion > Legacy_Settings.EngineVersion.TT && Legacy_Settings.s.game != Legacy_Settings.Game.LargoWinch) {
                if (Legacy_Settings.s.engineVersion < Legacy_Settings.EngineVersion.R3) {
                    po.off_collideBoundingVolume = po.off_visualBoundingVolume;
                    reader.ReadUInt32();
                } else {
                    po.off_collideBoundingVolume = LegacyPointer.Read(reader);
                }
            }

            // Parse visual set
            LegacyPointer.DoAt(ref reader, po.off_visualSet, () => {
                ushort numberOfLOD = 1;
                po.visualSetType = 0;
				if (Legacy_Settings.s.game == Legacy_Settings.Game.LargoWinch) {
					po.visualSet = new VisualSetLOD[1];
					po.visualSet[0] = new VisualSetLOD();
					po.visualSet[0].obj = null;
					po.visualSet[0].off_data = po.off_visualSet;
					po.visualSet[0].LODdistance = 5f;
				} else if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
					po.visualSet = new VisualSetLOD[1];
					po.visualSet[0] = new VisualSetLOD();
					po.visualSet[0].obj = MapLoader.Loader.meshObjects.FirstOrDefault(p => p.offset == po.off_visualSet);
					po.visualSet[0].off_data = po.off_visualSet;
					po.visualSet[0].LODdistance = 5f;
				} else {
					if (Legacy_Settings.s.platform != Legacy_Settings.Platform.DC 
					|| Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman2DCDevBuild_1999_11_22) {
						reader.ReadUInt32(); // 0
						numberOfLOD = reader.ReadUInt16();
						//if (numberOfLOD > 1) MapLoader.Loader.print("Found a PO with " + numberOfLOD + " levels of detail @ " + offset);
						po.visualSetType = reader.ReadUInt16();
						if (numberOfLOD > 0) {
							LegacyPointer off_LODDistances = LegacyPointer.Read(reader);
							LegacyPointer off_LODDataOffsets = LegacyPointer.Read(reader);
							reader.ReadUInt32(); // always 0? RLI table offset
							if (Legacy_Settings.s.engineVersion > Legacy_Settings.EngineVersion.Montreal) reader.ReadUInt32(); // always 0? number of RLI
							po.visualSet = new VisualSetLOD[numberOfLOD];
							for (uint i = 0; i < numberOfLOD; i++) {
								po.visualSet[i] = new VisualSetLOD();
							}
							LegacyPointer.DoAt(ref reader, off_LODDistances, () => {
								for (uint i = 0; i < numberOfLOD; i++) {
									// if distance > the float at this offset, game engine uses next LOD if there is one
									po.visualSet[i].LODdistance = reader.ReadSingle();
								}
							});
							LegacyPointer.DoAt(ref reader, off_LODDataOffsets, () => {
								for (uint i = 0; i < numberOfLOD; i++) {
									po.visualSet[i].off_data = LegacyPointer.Read(reader);
								}
							});
						}
					} else {
						// Platform = Dreamcast
						LegacyPointer.Read(reader); // Material pointer?
						LegacyPointer off_data = LegacyPointer.Read(reader);
						reader.ReadUInt32(); // always 0?
						reader.ReadUInt32(); // always 0?
						po.visualSet = new VisualSetLOD[1];
						po.visualSet[0].off_data = off_data;
						po.visualSet[0].LODdistance = 5f;
					}
				}
                int radiosityLODIndex = 0;
                for (uint i = 0; i < numberOfLOD; i++) {
                    LegacyPointer.DoAt(ref reader, po.visualSet[i].off_data, () => {
                        switch (po.visualSetType) {
                            case 0:
                                if(po.visualSet[i].obj == null) po.visualSet[i].obj = GeometricObject.Read(reader, po.visualSet[i].off_data, radiosity: radiosity?.lod?[radiosityLODIndex++]);
                                break;
                            case 1:
								if (po.visualSet[i].obj == null) po.visualSet[i].obj = PatchGeometricObject.Read(reader, po, po.visualSet[i].off_data);
                                break;
                            default:
                                MapLoader.Loader.print("unknown type " + po.visualSetType + " at offset: " + offset);
                                break;
                        }
                    });
                }
            });

            // Parse collide set
            LegacyPointer.DoAt(ref reader, po.off_collideSet, () => {
				if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
					// Read collide mesh object here directly
					po.collideMesh = GeometricObjectCollide.Read(reader, po.off_collideSet);
				} else {
					// Read collide set containing collide mesh
					uint u1 = reader.ReadUInt32(); // 0, zdm
					uint u2 = reader.ReadUInt32(); // 0, zdd
					uint u3 = reader.ReadUInt32(); // 0, zde
					LegacyPointer off_zdr = LegacyPointer.Read(reader);
					LegacyPointer.DoAt(ref reader, off_zdr, () => {
						po.collideMesh = GeometricObjectCollide.Read(reader, off_zdr);
					});
				}
            });
            MapLoader.Loader.physicalObjects.Add(po);
            return po;
        }

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public PhysicalObject Clone() {
            PhysicalObject po = (PhysicalObject)MemberwiseClone();
            po.visualSet = new VisualSetLOD[visualSet.Length];
            po.Reset();
            for (int i = 0; i < visualSet.Length; i++) {
                po.visualSet[i].LODdistance = visualSet[i].LODdistance;
                po.visualSet[i].off_data = visualSet[i].off_data;
                po.visualSet[i].obj = visualSet[i].obj.Clone();
            }
            if (collideMesh != null) {
                po.collideMesh = collideMesh.Clone();
            }
            return po;
        }

        public void Destroy() {
			//MapLoader.Loader.physicalObjects.Remove(this);
			if (visualSet != null) visualSet = null;
			if (collideMesh != null) collideMesh = null;
			if (gao != null) GameObject.Destroy(gao);
        }

		public void UpdateViewCollision(bool viewCollision) {
            if (gao == null) return;
			foreach (VisualSetLOD l in visualSet) {
				if (l.obj != null) {
					GameObject gao = l.obj.Gao;
					if (gao != null) gao.SetActive(!viewCollision);
				}
			}

            bool spoHasNoCollisionFlag = superObject?.flags.HasFlag(SuperObjectFlags.Flags.NoCollision) ?? false;
            var parentSector = superObject?.ParentSector;
            bool isInVirtualSector = parentSector != null && (parentSector.data as Sector)?.isSectorVirtual > 0;

            collideMesh?.SetVisualsActive(viewCollision && ((!spoHasNoCollisionFlag && !isInVirtualSector) || UnitySettings.ShowCollisionDataForNoCollisionObjects));
        }
    }
}
