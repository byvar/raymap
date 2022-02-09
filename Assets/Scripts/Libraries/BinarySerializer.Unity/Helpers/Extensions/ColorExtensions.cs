using System;
using BinarySerializer;
using System.Collections.Generic;
using UnityEngine;

namespace BinarySerializer.Unity {
    public static class ColorExtensions {
        public static Color GetColor(this BaseColor c) {
            var context = c.Context;

            // If we have no context we can't cache it, so just convert and return
            if (context == null)
                return ColorCache.ToColor(c);

            // Get or create a cache for the colors
            var colorCache = context.GetStoredObject<ColorCache>(ColorCache.ColorCacheKey) ?? context.StoreObject(ColorCache.ColorCacheKey, new ColorCache(context));

            // Return the color
            return colorCache.CacheOrGetColor(c);
        }

        public static CustomColor GetColor(this Color c) {
            return new CustomColor(c.r, c.g, c.b, c.a);
        }

        public class ColorCache {
            public ColorCache(Context context) {
                Context = context;
                Cache = new Dictionary<BaseColor, Color>();

                Context.Disposed += Context_Disposed;
            }

            public const string ColorCacheKey = "CachedColors";

            public Context Context { get; }
            public Dictionary<BaseColor, Color> Cache { get; }

            public Color CacheOrGetColor(BaseColor c) {
                // If the color is not cached we create it
                if (!Cache.ContainsKey(c)) {
                    // Convert the color
                    var color = ToColor(c);

                    // Cache the color
                    Cache.Add(c, color);

                    // Make sure to update the cached color if the color is modified
                    c.ColorModified += C_ColorModified;

                    return color;
                } else {
                    // Return cached color
                    return Cache[c];
                }
            }

            private void C_ColorModified(object sender, EventArgs e) {
                var c = (BaseColor)sender;
                Cache[c] = ToColor(c);
            }

            private void Context_Disposed(object sender, EventArgs e) {
                // Remove this from the cache
                Context.RemoveStoredObject(ColorCacheKey);

                // Unsubscribe to all events
                foreach (var c in Cache.Keys)
                    c.ColorModified -= C_ColorModified;

                // Clear cache
                Cache.Clear();
            }

            public static Color ToColor(BaseColor c) => new Color(c.Red, c.Green, c.Blue, c.Alpha);
        }
    }
}