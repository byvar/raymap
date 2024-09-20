using BinarySerializer.PlayStation.PS1;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_Triangle : BinarySerializable, GEO_IPS1Polygon
	{
		public ushort V0 { get; set; }
		public ushort V1 { get; set; }
		public ushort V2 { get; set; }
		public byte MaterialFlags { get; set; }
		public byte Scroll { get; set; }
		public byte X0 { get; set; }
		public byte Y0 { get; set; }
		public CBA CBA { get; set; }
		public byte X1 { get; set; }
		public byte Y1 { get; set; }
		public TSB TSB { get; set; }
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
			CBA = s.SerializeObject<CBA>(CBA, name: nameof(CBA));
			X1 = s.Serialize<byte>(X1, name: nameof(X1));
			Y1 = s.Serialize<byte>(Y1, name: nameof(Y1));
			TSB = s.SerializeObject<TSB>(TSB, name: nameof(TSB));
			X2 = s.Serialize<byte>(X2, name: nameof(X2));
			Y2 = s.Serialize<byte>(Y2, name: nameof(Y2));
			Ushort_12 = s.Serialize<ushort>(Ushort_12, name: nameof(Ushort_12));

			RegisterTexture();
		}
		#region GEO_IPS1Polygon implementation
		public GLI_Texture Texture => Context?.GetLevel()?.TextureCache?.GetTexture(TSB, CBA, X0, Y0);

		public GLI_VisualMaterial Material => new GLI_VisualMaterial(Context) {
			Texture = Texture,
			Scroll = Scroll,
			MaterialFlags = MaterialFlags,
		};

		public void RegisterTexture() {
			byte[] x = new[] { X0, X1, X2 };
			byte[] y = new[] { Y0, Y1, Y2 };
			int xMin = x.Min();
			int xMax = x.Max() + 1;
			int yMin = y.Min();
			int yMax = y.Max() + 1;
			int w = xMax - xMin;
			int h = yMax - yMin;
			Context?.GetLevel()?.TextureCache?.RegisterTexture(TSB, CBA, xMin, xMax, yMin, yMax);
		}
		#endregion
	}
}