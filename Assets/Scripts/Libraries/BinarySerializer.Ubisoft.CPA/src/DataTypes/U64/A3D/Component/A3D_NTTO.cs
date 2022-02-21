using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class A3D_NTTO : BinarySerializable
    {
        public ushort TypeOfObject { get; set; }
		public byte IndexInTable { get; set; }
		public byte Transparency { get; set; }

		// Parsed
		public ElementType Type { get; set; }
		public ElementTypeFlags TypeFlags { get; set; }


		public override void SerializeImpl(SerializerObject s)
        {
			TypeOfObject = s.Serialize<ushort>(TypeOfObject, name: nameof(TypeOfObject));
			s.DoAt(Offset, () => {
				s.DoBits<ushort>(b => {
					Type = b.SerializeBits<ElementType>(Type, 8, name: nameof(Type));
					TypeFlags = b.SerializeBits<ElementTypeFlags>(TypeFlags, 8, name: nameof(TypeFlags));
				});
			});
			IndexInTable = s.Serialize<byte>(IndexInTable, name: nameof(IndexInTable));
			Transparency = s.Serialize<byte>(Transparency, name: nameof(Transparency));
		}

		public enum ElementType {
			GraphicObject = 0,
			SubAnimation = 1,
			EmptyObject = 2,
			Event = 3,
			Light = 4,
			Fake = 5,
			Undefined = 6,
		}
		public enum ElementTypeFlags {
			WrapLastKey			= 1 << 1,
			ChangeOfHierarchy	= 1 << 2,
			Hierarchized		= 1 << 3,
			SoundEvent			= 1 << 4,
		}
    }
}