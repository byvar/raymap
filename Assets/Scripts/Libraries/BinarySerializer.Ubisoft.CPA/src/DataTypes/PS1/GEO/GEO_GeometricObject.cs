using System;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_GeometricObject : BinarySerializable
	{
		public uint Uint_00 { get; set; }
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

		// Serialized from pointers
		public GEO_Vertex[] Vertices { get; set; }
		public GEO_PolygonList[] PolygonLists { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
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
				// TODO: Implement
				throw new NotImplementedException();
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
		}
	}
}