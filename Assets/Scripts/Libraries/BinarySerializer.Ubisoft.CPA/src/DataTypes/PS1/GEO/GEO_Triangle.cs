using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_Triangle : BinarySerializable
	{
		public ushort V0 { get; set; }
		public ushort V1 { get; set; }
		public ushort V2 { get; set; }
		public byte MaterialFlags { get; set; }
		public byte Scroll { get; set; }
		public byte X0 { get; set; }
		public byte Y0 { get; set; }
		public PS1_CBA CBA { get; set; }
		public byte X1 { get; set; }
		public byte Y1 { get; set; }
		public PS1_TSB TSB { get; set; }
		public byte X2 { get; set; }
		public byte Y2 { get; set; }
		public ushort Ushort_12 { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			V1 = s.Serialize<ushort>(V1, name: nameof(V1));
			V2 = s.Serialize<ushort>(V2, name: nameof(V2));
			MaterialFlags = s.Serialize<byte>(MaterialFlags, name: nameof(MaterialFlags));
			Scroll = s.Serialize<byte>(Scroll, name: nameof(Scroll));
			X0 = s.Serialize<byte>(X0, name: nameof(X0));
			Y0 = s.Serialize<byte>(Y0, name: nameof(Y0));
			CBA = s.SerializeObject<PS1_CBA>(CBA, name: nameof(CBA));
			X1 = s.Serialize<byte>(X1, name: nameof(X1));
			Y1 = s.Serialize<byte>(Y1, name: nameof(Y1));
			TSB = s.SerializeObject<PS1_TSB>(TSB, name: nameof(TSB));
			X2 = s.Serialize<byte>(X2, name: nameof(X2));
			Y2 = s.Serialize<byte>(Y2, name: nameof(Y2));
			Ushort_12 = s.Serialize<ushort>(Ushort_12, name: nameof(Ushort_12));
		}
	}
}