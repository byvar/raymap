namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_Vector : BinarySerializable
	{
		public short X { get; set; }
		public short Y { get; set; }
		public short Z { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));
		}
	}
}