using BinarySerializer.PS1;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_Quad : BinarySerializable, GEO_IPS1Polygon
	{
		public ushort V0 { get; set; }
		public ushort V1 { get; set; }
		public ushort V2 { get; set; }
		public ushort V3 { get; set; }
		public byte MaterialFlags { get; set; }
		public byte Scroll { get; set; }
		public ushort Ushort_0A { get; set; }
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
			V0 = s.Serialize<ushort>(V0, name: nameof(V0));
			V1 = s.Serialize<ushort>(V1, name: nameof(V1));
			V2 = s.Serialize<ushort>(V2, name: nameof(V2));
			V3 = s.Serialize<ushort>(V3, name: nameof(V3));
			MaterialFlags = s.Serialize<byte>(MaterialFlags, name: nameof(MaterialFlags));
			Scroll = s.Serialize<byte>(Scroll, name: nameof(Scroll));
			Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
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
			byte[] x = new[] { X0, X1, X2, X3 };
			byte[] y = new[] { Y0, Y1, Y2, Y3 };
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