using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_Sprite : BinarySerializable
	{
		public byte MaterialFlags { get; set; }
		public byte Scroll { get; set; }
		public ushort Width { get; set; }
		public ushort V0 { get; set; }
		public ushort Height { get; set; }
		public byte X0 { get; set; }
		public byte Y0 { get; set; }
		public PS1_CBA CBA { get; set; }
		public byte X1 { get; set; }
		public byte Y1 { get; set; }
		public PS1_TSB TSB { get; set; }
		public byte X2 { get; set; }
		public byte Y2 { get; set; }
		public byte X3 { get; set; }
		public byte Y3 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			MaterialFlags = s.Serialize<byte>(MaterialFlags, name: nameof(MaterialFlags));
			Scroll = s.Serialize<byte>(Scroll, name: nameof(Scroll));
			Width = s.Serialize<ushort>(Width, name: nameof(Width));
			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			Height = s.Serialize<ushort>(Height, name: nameof(Height));
			X0 = s.Serialize<byte>(X0, name: nameof(X0));
			Y0 = s.Serialize<byte>(Y0, name: nameof(Y0));
			CBA = s.SerializeObject<PS1_CBA>(CBA, name: nameof(CBA));
			X1 = s.Serialize<byte>(X1, name: nameof(X1));
			Y1 = s.Serialize<byte>(Y1, name: nameof(Y1));
			TSB = s.SerializeObject<PS1_TSB>(TSB, name: nameof(TSB));
			X2 = s.Serialize<byte>(X2, name: nameof(X2));
			Y2 = s.Serialize<byte>(Y2, name: nameof(Y2));
			X3 = s.Serialize<byte>(X3, name: nameof(X3));
			Y3 = s.Serialize<byte>(Y3, name: nameof(Y3));
		}
	}
}