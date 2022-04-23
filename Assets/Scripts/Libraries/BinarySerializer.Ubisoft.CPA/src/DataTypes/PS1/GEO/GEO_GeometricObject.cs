using System;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_GeometricObject : BinarySerializable
	{
		public uint UInt_00 { get; set; }
		public ushort VerticesCount { get; set; }
		public ushort PolygonListsCount { get; set; }
		public short Short_08 { get; set; }
		public short Short_0A { get; set; }
		public short Short_0C { get; set; }
		public ushort Ushort_0E { get; set; }
		public Pointer VerticesPointer { get; set; }
		public Pointer PolygonListsPointer { get; set; }
		public short Short_18 { get; set; }
		public short Short_1A { get; set; }
		public short CurrentScrollValue { get; set; }
		public short Short_1E { get; set; }

		// VIP, Jungle Book
		public ushort BonesCount { get; set; }
		public ushort BoneWeightsCount { get; set; }
		public ushort BonesUnknownCount { get; set; }
		public ushort Unknown4Count { get; set; }
		public Pointer BonesPointer { get; set; }
		public Pointer BoneWeightsPointer { get; set; }
		public Pointer BonesUnknownPointer { get; set; }
		public Pointer Unknown4Pointer { get; set; }

		// Serialized from pointers
		public GEO_Vertex[] Vertices { get; set; }
		public GEO_PolygonList[] PolygonLists { get; set; }

		public GEO_DeformationBone[] Bones { get; set; }
		public GEO_DeformationVertexWeightSet[] BoneWeights { get; set; }
		public GEO_DeformationUnknown[] BonesUnknown { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			VerticesCount = s.Serialize<ushort>(VerticesCount, name: nameof(VerticesCount));
			PolygonListsCount = s.Serialize<ushort>(PolygonListsCount, name: nameof(PolygonListsCount));
			Short_08 = s.Serialize<short>(Short_08, name: nameof(Short_08));
			Short_0A = s.Serialize<short>(Short_0A, name: nameof(Short_0A));
			Short_0C = s.Serialize<short>(Short_0C, name: nameof(Short_0C));
			Ushort_0E = s.Serialize<ushort>(Ushort_0E, name: nameof(Ushort_0E));
			VerticesPointer = s.SerializePointer(VerticesPointer, name: nameof(VerticesPointer));
			PolygonListsPointer = s.SerializePointer(PolygonListsPointer, name: nameof(PolygonListsPointer));

			if (settings.EngineVersion == EngineVersion.VIP_PS1 || settings.EngineVersion == EngineVersion.JungleBook_PS1)
			{
				BonesCount = s.Serialize<ushort>(BonesCount, name: nameof(BonesCount));
				BoneWeightsCount = s.Serialize<ushort>(BoneWeightsCount, name: nameof(BoneWeightsCount));
				BonesUnknownCount = s.Serialize<ushort>(BonesUnknownCount, name: nameof(BonesUnknownCount));
				Unknown4Count = s.Serialize<ushort>(Unknown4Count, name: nameof(Unknown4Count));

				BonesPointer = s.SerializePointer(BonesPointer, allowInvalid: BonesCount == 0, name: nameof(BonesPointer));
				BoneWeightsPointer = s.SerializePointer(BoneWeightsPointer, allowInvalid: BoneWeightsCount == 0, name: nameof(BoneWeightsPointer));
				BonesUnknownPointer = s.SerializePointer(BonesUnknownPointer, allowInvalid: BonesUnknownCount == 0, name: nameof(BonesUnknownPointer));
				Unknown4Pointer = s.SerializePointer(Unknown4Pointer, allowInvalid: Unknown4Count == 0, name: nameof(Unknown4Pointer));
			}
			else
			{
				Short_18 = s.Serialize<short>(Short_18, name: nameof(Short_18));
				Short_1A = s.Serialize<short>(Short_1A, name: nameof(Short_1A));
				CurrentScrollValue = s.Serialize<short>(CurrentScrollValue, name: nameof(CurrentScrollValue));
				Short_1E = s.Serialize<short>(Short_1E, name: nameof(Short_1E));
			}

			// Serialize data from pointers
			s.DoAt(VerticesPointer, () => 
				Vertices = s.SerializeObjectArray<GEO_Vertex>(Vertices, VerticesCount, name: nameof(Vertices)));
			s.DoAt(PolygonListsPointer, () => 
				PolygonLists = s.SerializeObjectArray<GEO_PolygonList>(PolygonLists, PolygonListsCount, name: nameof(PolygonLists)));

			s.DoAt(BonesPointer, () =>
				Bones = s.SerializeObjectArray<GEO_DeformationBone>(Bones, BonesCount, name: nameof(Bones)));
			s.DoAt(BoneWeightsPointer, () =>
				BoneWeights = s.SerializeObjectArray<GEO_DeformationVertexWeightSet>(BoneWeights, BoneWeightsCount, name: nameof(BoneWeights)));
			s.DoAt(BonesUnknownPointer, () =>
				BonesUnknown = s.SerializeObjectArray<GEO_DeformationUnknown>(BonesUnknown, BonesUnknownCount, name: nameof(BonesUnknown)));
		}
	}
}