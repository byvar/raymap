using OpenSpace.Object;
using System.Collections.Generic;

namespace OpenSpace.Collide {
    public class CollSet {
        public Pointer offset;
        public Perso perso;

		// Struct
		public Dictionary<CollideType, Pointer> off_zdxList = new Dictionary<CollideType, Pointer>();
		public Dictionary<CollideType, Pointer> off_activationList = new Dictionary<CollideType, Pointer>();
		public Dictionary<CollideType, Pointer> off_zones = new Dictionary<CollideType, Pointer>();
		public Dictionary<CollideType, Pointer> off_currentZone = new Dictionary<CollideType, Pointer>();

		// consists of 16 bit pairs that describe the state of a zone, 00 = neutral, 01 = force active, 10 = force inactive
		// access these using GetPrivilegedActionZoneStatus
		public Dictionary<CollideType, int> privilegedActivations = new Dictionary<CollideType, int>(); 

		// Generated
		public Dictionary<CollideType, LinkedList<GeometricObjectCollide>> zdxList = new Dictionary<CollideType, LinkedList<GeometricObjectCollide>>();
		public Dictionary<CollideType, LinkedList<CollideActivation>> activationList = new Dictionary<CollideType, LinkedList<CollideActivation>>();
		public Dictionary<CollideType, LinkedList<CollideActivationZone>> zones = new Dictionary<CollideType, LinkedList<CollideActivationZone>>();

        public CollSet(Perso perso, Pointer offset) {
            this.perso = perso;
            this.offset = offset;
        }

        public enum PrivilegedActivationStatus {
            Neutral = 0,
            ForceActive = 1,
            ForceInactive = 2
        }

        public PrivilegedActivationStatus GetPrivilegedActionZoneStatus(CollideType type, int index)
        {
            int activations = 0;
			if (privilegedActivations.ContainsKey(type)) {
				activations = privilegedActivations[type];
			}
            int offset = index * 2;
            int value = ((1 << 2) - 1) & (activations >> (offset)); // extract 2 bits from offset
            return (PrivilegedActivationStatus)value;
        }

        public static CollSet Read(Reader reader, Perso perso, Pointer offset) {
            MapLoader l = MapLoader.Loader;
			//if (Settings.s.platform == Settings.Platform.DC) return null;
			//l.print("CollSet @ " + offset);
            CollSet c = new CollSet(perso, offset);

			c.off_zdxList[CollideType.ZDD] = Pointer.Read(reader);
			c.off_zdxList[CollideType.ZDE] = Pointer.Read(reader);
			c.off_zdxList[CollideType.ZDM] = Pointer.Read(reader);
			c.off_zdxList[CollideType.ZDR] = Pointer.Read(reader);

			c.off_activationList[CollideType.ZDD] = Pointer.Read(reader);
			c.off_activationList[CollideType.ZDE] = Pointer.Read(reader);
			c.off_activationList[CollideType.ZDM] = Pointer.Read(reader);
			c.off_activationList[CollideType.ZDR] = Pointer.Read(reader);

			c.off_zones[CollideType.ZDD] = Pointer.Read(reader);
			c.off_zones[CollideType.ZDE] = Pointer.Read(reader);
			c.off_zones[CollideType.ZDM] = Pointer.Read(reader);
			c.off_zones[CollideType.ZDR] = Pointer.Read(reader);

			if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
				c.privilegedActivations[CollideType.ZDD] = reader.ReadInt32();
				c.privilegedActivations[CollideType.ZDE] = reader.ReadInt32();
				c.privilegedActivations[CollideType.ZDM] = reader.ReadInt32();
				c.privilegedActivations[CollideType.ZDR] = reader.ReadInt32();
			}

			foreach (KeyValuePair<CollideType, Pointer> entry in c.off_zdxList) {
				Pointer.DoAt(ref reader, entry.Value, () => {
					//zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
					c.zdxList[entry.Key] = LinkedList<GeometricObjectCollide>.Read(ref reader, entry.Value,
						(off_element) => {
							GeometricObjectCollide col = GeometricObjectCollide.Read(reader, off_element, type: entry.Key);
							col.gao.transform.SetParent(perso.Gao.transform);
							return col;
						},
						flags: LinkedList.Flags.ReadAtPointer
							| (Settings.s.hasLinkedListHeaderPointers ?
								LinkedList.Flags.HasHeaderPointers :
								LinkedList.Flags.NoPreviousPointersForDouble),
						type: LinkedList.Type.Minimize
					);
				});
			}
			foreach (KeyValuePair<CollideType, Pointer> entry in c.off_zones) {
				Pointer.DoAt(ref reader, entry.Value, () => {
					//zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
					c.zones[entry.Key] = LinkedList<CollideActivationZone>.Read(ref reader, entry.Value,
						(off_element) => {
							return CollideActivationZone.Read(reader, off_element);
						},
						flags: (Settings.s.hasLinkedListHeaderPointers ?
								LinkedList.Flags.HasHeaderPointers :
								LinkedList.Flags.NoPreviousPointersForDouble),
						type: LinkedList.Type.Minimize
					);
				});
			}
			foreach (KeyValuePair<CollideType, Pointer> entry in c.off_activationList) {
				Pointer.DoAt(ref reader, entry.Value, () => {
					//zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
					c.activationList[entry.Key] = LinkedList<CollideActivation>.Read(ref reader, entry.Value,
						(off_element) => {
							return CollideActivation.Read(reader, off_element, c, entry.Key);
						},
						flags: (Settings.s.hasLinkedListHeaderPointers ?
								LinkedList.Flags.HasHeaderPointers :
								LinkedList.Flags.NoPreviousPointersForDouble),
						type: LinkedList.Type.Minimize
					);
				});
			}

            return c;
        }



		public CollSet Clone(UnityEngine.Transform parent) {
			CollSet m = (CollSet)MemberwiseClone();
			m.zdxList = new Dictionary<CollideType, LinkedList<GeometricObjectCollide>>();
			foreach (KeyValuePair<CollideType, LinkedList<GeometricObjectCollide>> kv in zdxList) {
				LinkedList<GeometricObjectCollide> geos = new LinkedList<GeometricObjectCollide>(kv.Value.offset, kv.Value.off_head, kv.Value.off_tail, (uint)kv.Value.Count, kv.Value.type);
				for (int i = 0; i < kv.Value.Count; i++) {
					geos[i] = kv.Value[i].Clone();
					geos[i].gao.transform.SetParent(parent);
				}
				m.zdxList.Add(kv.Key, geos);
			}
			return m;
		}
	}
}