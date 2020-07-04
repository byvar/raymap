using UnityEngine;
using UnityEditor;

namespace OpenSpace.Visual.ISI {
    public class ColorISI : OpenSpaceStruct {
		public short r;
		public short g;
		public short b;
		public short a;

		/*public Color Color => new Color(
			Mathf.Max(0, (float)r / short.MaxValue),
			Mathf.Max(0, (float)g / short.MaxValue),
			Mathf.Max(0, (float)b / short.MaxValue),
			Mathf.Max(0, (float)a / short.MaxValue));*/
		public Color Color => new Color(
			Mathf.Clamp01((float)r / 256f),// - 0.5f + Settings.s.luminosity),
			Mathf.Clamp01((float)g / 256f),// - 0.5f + Settings.s.luminosity),
			Mathf.Clamp01((float)b / 256f),// - 0.5f + Settings.s.luminosity),
			Mathf.Clamp01((float)a / 256f));

		protected override void ReadInternal(Reader reader) {
			r = reader.ReadInt16();
			g = reader.ReadInt16();
			b = reader.ReadInt16();
			a = reader.ReadInt16();
		}
	}
}