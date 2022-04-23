using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_DeformationBone : BinarySerializable {
		public ushort VerticesCount { get; set; }
		public short Short_02 { get; set; }
		public MTH3D_Vector_PS1_Short TranslationVector { get; set; }
		public short Short_0A { get; set; }
		public Pointer VerticesPointer { get; set; }
		public MTH3D_Matrix_PS1 RotationMatrix { get; set; }

		public ushort[] Unknown { get; set; }
		public ushort[] Vertices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VerticesCount = s.Serialize<ushort>(VerticesCount, name: nameof(VerticesCount));
			Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
			TranslationVector = s.SerializeObject<MTH3D_Vector_PS1_Short>(TranslationVector, name: nameof(TranslationVector));
			Short_0A = s.Serialize<short>(Short_0A, name: nameof(Short_0A));
			VerticesPointer = s.SerializePointer(VerticesPointer, allowInvalid: VerticesCount == 0, name: nameof(VerticesPointer));
			RotationMatrix = s.SerializeObject<MTH3D_Matrix_PS1>(RotationMatrix, name: nameof(RotationMatrix));

			Unknown = s.SerializeArray<ushort>(Unknown, 7, name: nameof(Unknown));

			s.DoAt(VerticesPointer, () =>
				Vertices = s.SerializeArray<ushort>(Vertices, VerticesCount, name: nameof(Vertices)));
		}
	}
}