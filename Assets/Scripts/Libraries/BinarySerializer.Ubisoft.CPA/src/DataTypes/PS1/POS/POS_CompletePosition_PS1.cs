namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class POS_CompletePosition_PS1 : BinarySerializable, MAT_ITransform
	{
		public MTH3D_Matrix_PS1 RotationMatrix { get; set; }
		public ushort Ushort_12 { get; set; }

		public MTH3D_Vector_PS1_Int TranslationVector { get; set; }
		public ushort Ushort_20 { get; set; }
		public ushort Ushort_22 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			RotationMatrix = s.SerializeObject<MTH3D_Matrix_PS1>(RotationMatrix, name: nameof(RotationMatrix));
			Ushort_12 = s.Serialize<ushort>(Ushort_12, name: nameof(Ushort_12));

			TranslationVector = s.SerializeObject<MTH3D_Vector_PS1_Int>(TranslationVector, name: nameof(TranslationVector));
			Ushort_20 = s.Serialize<ushort>(Ushort_20, name: nameof(Ushort_20));
			Ushort_22 = s.Serialize<ushort>(Ushort_22, name: nameof(Ushort_22));
		}

		#region MAT_ITransform implementation
		public MTH3D_Vector Position {
			get => new MTH3D_Vector(TranslationVector.X, TranslationVector.Y, TranslationVector.Z);
			set => TranslationVector = new MTH3D_Vector_PS1_Int(value.X, value.Y, value.Z);
		}
		public MTH4D_Vector Rotation {
			get => RotationMatrix.GetRotation();
			set => throw new System.NotImplementedException();
		}
		public MTH3D_Vector Scale {
			get => MTH3D_Vector.One;
			set => throw new System.NotImplementedException();
		}
		#endregion
	}
}