using NDSParse.Conversion.Textures.Colors.Types;
using NDSParse.Data;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Textures.Colors;

public static class ColorExtensions
{
    public static Color[] ReadColors<T>(this BaseReader reader, int count) where T : ColorTypeBase, new()
    {
        var decoder = new T();
        return decoder.Decode(reader, count);
    }
}