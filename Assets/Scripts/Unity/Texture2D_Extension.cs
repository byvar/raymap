using UnityEngine;

public static class Texture2D_Extension {
    public static bool HasAlpha(Color[] aColors) {
        for (int i = 0; i < aColors.Length; i+=4) // don't check everything lol
            if (aColors[i].a < 1f)
                return true;
        return false;
    }
    public static bool HasColor(Color[] aColors) {
        for (int i = 0; i < aColors.Length; i += 4) // don't check everything lol
            if (aColors[i].r != aColors[i].g || aColors[i].r != aColors[i].b)
                return true;
        return false;
    }

    public static bool HasAlpha(this Texture2D aTex) {
        return HasAlpha(aTex.GetPixels());
    }

    public static bool HasColor(this Texture2D aTex) {
        return HasColor(aTex.GetPixels());
    }
}