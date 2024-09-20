using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public interface GEO_IPS1Polygon
	{
		public Pointer Offset { get; }
		public void RegisterTexture();
		public GLI_Texture Texture { get; }
		public GLI_VisualMaterial Material { get; }
	}
}