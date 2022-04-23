using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public interface GEO_IPS1Polygon
	{
		public void RegisterTexture();
		public GLI_TextureBounds Texture { get; }
		public GLI_VisualMaterial Material { get; }
	}
}