using System.ComponentModel;

namespace LibGC.Texture
{
    /// <summary>
    /// Gx texture formats (along with TPL identifiers).
    /// </summary>
    public enum GcTextureFormat
    {
        [Description("4 bit grayscale")]
        I4 = 0,
        [Description("8 bit grayscale")]
        I8 = 1,
        [Description("4 bit grayscale & 4 bit transparency")]
        IA4 = 2,
        [Description("8 bit grayscale & 8 bit transparency")]
        IA8 = 3,
        [Description("16 bit color")]
        RGB565 = 4,
        [Description("16 bit color & transparency")]
        RGB5A3 = 5,
        [Description("24 bit color & 8 bit transparency")]
        RGBA8 = 6,
        [Description("16 color palette")]
        CI4 = 8,
        [Description("256 color palette")]
        CI8 = 9,
        [Description("16384 color palette")]
        CI14X2 = 10,
        [Description("Compressed 16 bit color")]
        CMPR = 14
    };
}

