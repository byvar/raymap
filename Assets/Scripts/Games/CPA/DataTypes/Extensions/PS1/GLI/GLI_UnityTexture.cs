using BinarySerializer.Unity;
using Raymap;
using System.Linq;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class GLI_UnityTexture : Unity_Data<GLI_Texture> {
		private Texture2D CachedTexture { get; set; }
		public Texture2D Texture {
			get {
				if (CachedTexture == null) {
					CachedTexture = Context.GetLevel().VRAM.GetTexture(LinkedObject.Width, LinkedObject.Height, LinkedObject.TSB, LinkedObject.CBA, LinkedObject.xMin, LinkedObject.yMin);
				}
				return CachedTexture;
			}
		}
		private bool? CachedIsTransparent { get; set;}
		public bool IsTransparent {
			get {
				if (!CachedIsTransparent.HasValue) {
					var tex = Texture;
					CachedIsTransparent = tex?.IsTransparent();
				}
				return CachedIsTransparent.Value;
			}
		}
	}
}
