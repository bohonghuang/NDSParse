using NDSParse.Conversion.Textures.Pixels.Indexed;
using NDSParse.Conversion.Textures.Pixels.Indexed.Types;
using NDSParse.Data;

namespace NDSParse.Conversion.Textures.Pixels;

public static class PixelExtensions
{
    public static IPixel[] ReadPixels<T>(this BaseReader reader, int width, int height) where T : PixelTypeBase, new()
    {
        var pixelType = new T();
        var data = reader.ReadBytes(width * height * pixelType.BitsPerPixel / 8);
        return pixelType.Decode(data);
    }
}