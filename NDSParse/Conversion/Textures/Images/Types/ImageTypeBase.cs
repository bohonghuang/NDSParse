using NDSParse.Conversion.Textures.Pixels;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Textures.Images.Types;

public abstract class ImageTypeBase
{
    public int Width;
    public int Height;
    public IndexedPixel[] Pixels;

    public ImageTypeBase(int width, int height, IndexedPixel[] pixels)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
    }
}