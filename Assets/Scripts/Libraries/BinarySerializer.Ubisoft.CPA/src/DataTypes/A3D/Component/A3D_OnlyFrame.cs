namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_OnlyFrame : BinarySerializable {
		public ushort AngularSpeedQuaternionIndex { get; set; }
		public ushort SpeedVectorIndex { get; set; }

		public ushort HierarchyCouplesCount { get; set; }
		public ushort HierarchyCouplesStart { get; set; }

		public ushort DeformationsCount { get; set; }
		public ushort DeformationsStart { get; set; }

		public ushort SavedFrameIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			AngularSpeedQuaternionIndex = s.Serialize<ushort>(AngularSpeedQuaternionIndex, name: nameof(AngularSpeedQuaternionIndex));
			SpeedVectorIndex = s.Serialize<ushort>(SpeedVectorIndex, name: nameof(SpeedVectorIndex));

			HierarchyCouplesCount = s.Serialize<ushort>(HierarchyCouplesCount, name: nameof(HierarchyCouplesCount));
			HierarchyCouplesStart = s.Serialize<ushort>(HierarchyCouplesStart, name: nameof(HierarchyCouplesStart));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				DeformationsCount = s.Serialize<ushort>(DeformationsCount, name: nameof(DeformationsCount));
				DeformationsStart = s.Serialize<ushort>(DeformationsStart, name: nameof(DeformationsStart));
			} else if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.LargoWinch)) {
				DeformationsStart = s.Serialize<ushort>(DeformationsStart, name: nameof(DeformationsStart));
			}

			SavedFrameIndex = s.Serialize<ushort>(SavedFrameIndex, name: nameof(SavedFrameIndex));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)
				|| s.GetCPASettings().Branch == EngineBranch.U64) {
				s.SerializePadding(2, logIfNotNull: true);
			}
		}
	}
}