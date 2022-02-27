using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_GeometricObject : U64_Struct {
		public float Scale { get; set; }
		public float Radius { get; set; }
		public U64_ArrayReference<U64_ShortVector3D> CollisionPoints { get; set; }
		public U64_ArrayReference<U64_ShortVector3D> VisualPoints { get; set; }
		public U64_ArrayReference<U64_ShortVector3D> Normals { get; set; }
		public U64_ArrayReference<GEO_CollisionElementListEntry> CollisionElements { get; set; }
		public U64_ArrayReference<GEO_VisualElementListEntry> VisualElements { get; set; }
		public ushort CollisionPointsCount { get; set; }
		public ushort VisualPointsCount { get; set; }
		public ushort CollisionElementsCount { get; set; }
		public ushort VisualElementsCount { get; set; }
		public ushort EdgesCount { get; set; }
		public Symmetry SymmetryType { get; set; }
		public RLI RliFlag { get; set; }
		public LookAt LookAtFlag { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Scale = s.Serialize<float>(Scale, name: nameof(Scale));
			Radius = s.Serialize<float>(Radius, name: nameof(Radius));
			
			CollisionPoints = s.SerializeObject<U64_ArrayReference<U64_ShortVector3D>>(CollisionPoints, name: nameof(CollisionPoints));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2_3D)) {
				VisualPoints = s.SerializeObject<U64_ArrayReference<U64_ShortVector3D>>(VisualPoints, name: nameof(VisualPoints));
				Normals = s.SerializeObject<U64_ArrayReference<U64_ShortVector3D>>(Normals, name: nameof(Normals));
			} else {
				VisualPoints = CollisionPoints;
			}
			CollisionElements = s.SerializeObject<U64_ArrayReference<GEO_CollisionElementListEntry>>(CollisionElements, name: nameof(CollisionElements));
			VisualElements = s.SerializeObject<U64_ArrayReference<GEO_VisualElementListEntry>>(VisualElements, name: nameof(VisualElements));
			
			CollisionPointsCount = s.Serialize<ushort>(CollisionPointsCount, name: nameof(CollisionPointsCount));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2_3D)) {
				VisualPointsCount = s.Serialize<ushort>(VisualPointsCount, name: nameof(VisualPointsCount));
			} else {
				VisualPointsCount = CollisionPointsCount;
			}
			CollisionElementsCount = s.Serialize<ushort>(CollisionElementsCount, name: nameof(CollisionElementsCount));
			VisualElementsCount = s.Serialize<ushort>(VisualElementsCount, name: nameof(VisualElementsCount));
			
			EdgesCount = s.Serialize<ushort>(EdgesCount, name: nameof(EdgesCount));
			SymmetryType = s.Serialize<Symmetry>(SymmetryType, name: nameof(SymmetryType));
			RliFlag = s.Serialize<RLI>(RliFlag, name: nameof(RliFlag));
			LookAtFlag = s.Serialize<LookAt>(LookAtFlag, name: nameof(LookAtFlag));

			CollisionPoints?.Resolve(s, CollisionPointsCount);
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2_3D)) {
				VisualPoints?.Resolve(s, VisualPointsCount);
				Normals?.Resolve(s, VisualPointsCount);
			}

			VisualElements?.Resolve(s, VisualElementsCount);
			CollisionElements?.Resolve(s, CollisionElementsCount);

		}

		// TODO: Determine other flags
		[Flags] 
		public enum RLI : ushort {
			None = 0,
			UseRLI				= 0x1,
			SemiLookAt			= 0x0100,
			TransparentObject	= 0x4000,
		}

		public enum Symmetry : ushort {
			None = 0xFFFF,
			X = 0,
			Y = 1,
			Z = 2
		}

		public enum LookAt : ushort {
			Disabled = 0, // 3D
			Enabled = 1, // LookAt
			SemiLookAt = 2,
		}
	}
}
