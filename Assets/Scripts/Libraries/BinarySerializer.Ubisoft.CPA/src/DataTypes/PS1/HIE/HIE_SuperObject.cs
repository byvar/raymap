namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class HIE_SuperObject : BinarySerializable
	{
		public bool Pre_IsDynamic { get; set; }

		public uint TypeCode { get; set; }
		public int DataIndex { get; set; }
		public CPA_DoubleLinkedList<HIE_SuperObject> Children { get; set; } // TODO: Serialize children
		public Pointer NextBrotherPointer { get; set; }
		public Pointer PrevBrotherPointer { get; set; }
		public Pointer ParentPointer { get; set; }
		public Pointer Matrix1Pointer { get; set; }
		public Pointer Matrix2Pointer { get; set; }
		public short Short_28 { get; set; }
		public short Short_2A { get; set; }
		public short Short_2C { get; set; }
		public short Short_2E { get; set; }
		public short Short_30 { get; set; }
		public short Short_32 { get; set; }
		public short Short_34 { get; set; }
		public short Short_36 { get; set; }
		public uint Uint_38 { get; set; }
		public Pointer Pointer_38 { get; set; }
		public short Short_3C { get; set; }
		public short Short_3E { get; set; }
		public short Short_40 { get; set; }
		public short Short_42 { get; set; }
		public short Short_44 { get; set; }
		public short Short_46 { get; set; }
		public short Short_48 { get; set; }
		public short Short_4A { get; set; }

		// Serialized from pointers
		public HIE_SuperObject Parent { get; set; }
		public PS1_Matrix Matrix1 { get; set; }
		public PS1_Matrix Matrix2 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			TypeCode = s.Serialize<uint>(TypeCode, name: nameof(TypeCode));
			DataIndex = s.Serialize<int>(DataIndex, name: nameof(DataIndex));
			Children = s.SerializeObject<CPA_DoubleLinkedList<HIE_SuperObject>>(Children, name: nameof(Children));
			NextBrotherPointer = s.SerializePointer(NextBrotherPointer, name: nameof(NextBrotherPointer));
			PrevBrotherPointer = s.SerializePointer(PrevBrotherPointer, name: nameof(PrevBrotherPointer));
			ParentPointer = s.SerializePointer(ParentPointer, name: nameof(ParentPointer));
			Matrix1Pointer = s.SerializePointer(Matrix1Pointer, name: nameof(Matrix1Pointer));

			if (settings.EngineVersion != EngineVersion.RaymanRush_PS1)
			{
				Matrix2Pointer = s.SerializePointer(Matrix2Pointer, name: nameof(Matrix2Pointer));
				Short_28 = s.Serialize<short>(Short_28, name: nameof(Short_28));
				Short_2A = s.Serialize<short>(Short_2A, name: nameof(Short_2A));
				Short_2C = s.Serialize<short>(Short_2C, name: nameof(Short_2C));
				Short_2E = s.Serialize<short>(Short_2E, name: nameof(Short_2E));
				Short_30 = s.Serialize<short>(Short_30, name: nameof(Short_30));
				Short_32 = s.Serialize<short>(Short_32, name: nameof(Short_32));
				Short_34 = s.Serialize<short>(Short_34, name: nameof(Short_34));
				Short_36 = s.Serialize<short>(Short_36, name: nameof(Short_36));

				if (Pre_IsDynamic)
					Uint_38 = s.Serialize<uint>(Uint_38, name: nameof(Uint_38));
				else
					Pointer_38 = s.SerializePointer(Pointer_38, name: nameof(Pointer_38));

				Short_3C = s.Serialize<short>(Short_3C, name: nameof(Short_3C));
				Short_3E = s.Serialize<short>(Short_3E, name: nameof(Short_3E));
				Short_40 = s.Serialize<short>(Short_40, name: nameof(Short_40));
				Short_42 = s.Serialize<short>(Short_42, name: nameof(Short_42));
				Short_44 = s.Serialize<short>(Short_44, name: nameof(Short_44));
				Short_46 = s.Serialize<short>(Short_46, name: nameof(Short_46));
				Short_48 = s.Serialize<short>(Short_48, name: nameof(Short_48));
				Short_4A = s.Serialize<short>(Short_4A, name: nameof(Short_4A));
			}

			s.DoAt(ParentPointer, () => Parent = s.SerializeObject<HIE_SuperObject>(Parent, name: nameof(Parent)));
			s.DoAt(Matrix1Pointer, () => Matrix1 = s.SerializeObject<PS1_Matrix>(Matrix1, name: nameof(Matrix1)));
			s.DoAt(Matrix2Pointer, () => Matrix2 = s.SerializeObject<PS1_Matrix>(Matrix2, name: nameof(Matrix2)));
		}
	}
}