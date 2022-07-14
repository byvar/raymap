using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_KeyFrame : BinarySerializable {
		public MTH3D_Vector WorldPivotPosition { get; set; }
		public float DistanceMaster { get; set; } // = WorldPivotPositon.Magnitude

		public ushort FrameIndex { get; set; }
		public A3D_KeyFrame_Mask Mask { get; set; }

		public ushort QuaternionOrientationIndex { get; set; }
		public ushort QuaternionScaleIndex { get; set; }
		public ushort VectorScaleIndex { get; set; }
		public ushort VectorPositionIndex { get; set; }

		// Interpolation
		public ushort AngleBetweenQuaternionOrientation { get; set; }
		public ushort AngleBetweenQuaternionScale { get; set; }
		public ushort AngleBetweenObjectCenterPositions { get; set; }
		public FixedPointInt16 InterpolationParameter { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanArena)
				&& s.GetCPASettings().Branch != EngineBranch.U64) {
				WorldPivotPosition = s.SerializeObject<MTH3D_Vector>(WorldPivotPosition, name: nameof(WorldPivotPosition));
				DistanceMaster = s.Serialize<float>(DistanceMaster, name: nameof(DistanceMaster));
			}

			FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));
			Mask = s.Serialize<A3D_KeyFrame_Mask>(Mask, name: nameof(Mask));

			QuaternionOrientationIndex = s.Serialize<ushort>(QuaternionOrientationIndex, name: nameof(QuaternionOrientationIndex));
			QuaternionScaleIndex = s.Serialize<ushort>(QuaternionScaleIndex, name: nameof(QuaternionScaleIndex));
			VectorScaleIndex = s.Serialize<ushort>(VectorScaleIndex, name: nameof(VectorScaleIndex));
			VectorPositionIndex = s.Serialize<ushort>(VectorPositionIndex, name: nameof(VectorPositionIndex));

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanArena)
				&& s.GetCPASettings().Branch != EngineBranch.U64) {
				AngleBetweenQuaternionOrientation = s.Serialize<ushort>(AngleBetweenQuaternionOrientation, name: nameof(AngleBetweenQuaternionOrientation));
				AngleBetweenQuaternionScale = s.Serialize<ushort>(AngleBetweenQuaternionScale, name: nameof(AngleBetweenQuaternionScale));
				AngleBetweenObjectCenterPositions = s.Serialize<ushort>(AngleBetweenObjectCenterPositions, name: nameof(AngleBetweenObjectCenterPositions));
			}

			InterpolationParameter = s.SerializeObject<FixedPointInt16>(InterpolationParameter,
				onPreSerialize: p => p.Pre_PointPosition = 13,
				name: nameof(InterpolationParameter));
		}

		public static bool IsAligned(Context c) => 
			!c.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanArena) && c.GetCPASettings().Branch != EngineBranch.U64;
	}
}