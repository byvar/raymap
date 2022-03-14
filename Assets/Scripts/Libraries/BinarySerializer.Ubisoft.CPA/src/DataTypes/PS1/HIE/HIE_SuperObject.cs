namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class HIE_SuperObject : BinarySerializable
	{
		public uint TypeCode { get; set; }
		public int DataIndex { get; set; }
		public CPA_DoubleLinkedList<HIE_SuperObject> Children { get; set; }
		public Pointer NextBrotherPointer { get; set; }
		public Pointer PrevBrotherPointer { get; set; }
		public Pointer ParentPointer { get; set; }
		public Pointer Matrix1Pointer { get; set; }

		// Serialized from pointers
		public PS1_Matrix Matrix1 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			TypeCode = s.Serialize<uint>(TypeCode, name: nameof(TypeCode));
			DataIndex = s.Serialize<int>(DataIndex, name: nameof(DataIndex));
			Children = s.SerializeObject<CPA_DoubleLinkedList<HIE_SuperObject>>(Children, name: nameof(Children));
			NextBrotherPointer = s.SerializePointer(NextBrotherPointer, name: nameof(NextBrotherPointer));
			PrevBrotherPointer = s.SerializePointer(PrevBrotherPointer, name: nameof(PrevBrotherPointer));
			ParentPointer = s.SerializePointer(ParentPointer, name: nameof(ParentPointer));
			Matrix1Pointer = s.SerializePointer(Matrix1Pointer, name: nameof(Matrix1Pointer));
			// TODO: Serialize remaining data


			s.DoAt(Matrix1Pointer, () => Matrix1 = s.SerializeObject<PS1_Matrix>(Matrix1, name: nameof(Matrix1)));
		}
	}
}