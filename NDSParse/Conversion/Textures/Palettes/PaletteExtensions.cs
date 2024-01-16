using NDSParse.Conversion.Textures.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Textures.Palettes;

public static class PaletteExtensions
{
    public static Image<Rgba32> ToImage(this Palette palette)
    {
        var bitmap = new Image<Rgba32>(palette.Colors.Count, 1);
        bitmap.IteratePixels((ref Rgba32 pixel, int index) =>
        {
            pixel = palette.Colors[index].ToPixel<Rgba32>();
        });

        return bitmap;
    }
}