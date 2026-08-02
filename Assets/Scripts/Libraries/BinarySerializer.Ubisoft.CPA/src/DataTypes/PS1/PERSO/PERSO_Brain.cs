using System;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PERSO_Brain : BinarySerializable {
		public byte[] Bytes_00 { get; set; }
		public Pointer Pointer_0C { get; set; }
		public Pointer Pointer_10 { get; set; }
		public uint Pointer_14 { get; set; } // Code?

		public override void SerializeImpl(SerializerObject s)
		{
			Bytes_00 = s.SerializeArray<byte>(Bytes_00, 12, name: nameof(Bytes_00));
			Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
			Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));
			Pointer_14 = s.Serialize<uint>(Pointer_14, name: nameof(Pointer_14));
		}
	}
}