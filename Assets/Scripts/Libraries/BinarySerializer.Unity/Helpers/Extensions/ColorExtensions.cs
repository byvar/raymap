using System;
using BinarySerializer;
using System.Collections.Generic;
using UnityEngine;

namespace BinarySerializer.Unity {
    public static class ColorExtensions {
        public static Color GetColor(this BaseColor c) {
            return new Color(c.Red, c.Green, c.Blue, c.Alpha);
		}

		public static CustomColor GetColor(this Color c) {
            return new CustomColor(c.r, c.g, c.b, c.a);
        }
    }
}