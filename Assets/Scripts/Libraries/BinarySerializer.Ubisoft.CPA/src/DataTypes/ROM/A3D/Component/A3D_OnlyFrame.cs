using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class A3D_OnlyFrame : BinarySerializable
    {
		public ushort AngularSpeedQuaternionIndex { get; set; }
		public ushort SpeedVectorIndex { get; set; }
		public ushort HierarchiesCount { get; set; }
		public ushort FirstHierarchyIndex { get; set; }
		public ushort SavedFrameIndex { get; set; } // NumOfNTTO / A3D_Frame
		public ushort Align { get; set; } // Commented out in source?

		public override void SerializeImpl(SerializerObject s)
        {
			AngularSpeedQuaternionIndex = s.Serialize<ushort>(AngularSpeedQuaternionIndex, name: nameof(AngularSpeedQuaternionIndex));
			SpeedVectorIndex = s.Serialize<ushort>(SpeedVectorIndex, name: nameof(SpeedVectorIndex));
			HierarchiesCount = s.Serialize<ushort>(HierarchiesCount, name: nameof(HierarchiesCount));
			FirstHierarchyIndex = s.Serialize<ushort>(FirstHierarchyIndex, name: nameof(FirstHierarchyIndex));
			SavedFrameIndex = s.Serialize<ushort>(SavedFrameIndex, name: nameof(SavedFrameIndex));
			Align = s.Serialize<ushort>(Align, name: nameof(Align));
		}
    }
}