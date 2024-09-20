using System;
using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class GAM_Level_PS1 {
		public Context Context { get; set; }

		public float CoordinateFactor { get; set; } = 100f;
		public GAM_GlobalPointerTable GlobalPointerTable { get; set; }
		public GLI_TextureCache TextureCache { get; set; }
		public VRAM VRAM { get; set; }

		public GAM_Level_PS1(Context c) {
			Context = c;
			c.StoreObject<GAM_Level_PS1>(ContextKey, this);

			TextureCache = new GLI_TextureCache(Context);
		}

		public static string ContextKey => nameof(GAM_Level_PS1);
	}
}
