using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Conversion.Textures;

public static class TextureExtensions
{
    public static byte BitsPerPixel(this TextureFormat format)
    {
        return format switch
        {
            TextureFormat.None => 0,
            TextureFormat.A3I5 => 8,
            TextureFormat.Color4 => 2,
            TextureFormat.Color16 => 4,
            TextureFormat.Color256 => 8,
            TextureFormat.Texel => 2,
            TextureFormat.A5I3 => 8,
            TextureFormat.A1BGR5 => 16
        };
    }
    
    public static int PaletteSize(this TextureFormat format)
    {
        return format switch
        {
            TextureFormat.None => 0,
            TextureFormat.A3I5 => 64,
            TextureFormat.Color4 => 8,
            TextureFormat.Color16 => 32,
            TextureFormat.Color256 => 512,
            TextureFormat.Texel => 512,
            TextureFormat.A5I3 => 16,
            TextureFormat.A1BGR5 => 0
        };
    }
    
    public static bool IsIndexed(this TextureFormat format)
    {
        return format != TextureFormat.A1BGR5 && format != TextureFormat.Texel;
    }
}

public enum TextureFormat : byte
{
    None = 0,
    A3I5 = 1,
    Color4 = 2,
    Color16 = 3,
    Color256 = 4,
    Texel = 5,
    A5I3 = 6,
    A1BGR5 = 7
}