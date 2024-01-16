using IPixel = NDSParse.Conversion.Textures.Pixels.IPixel;

namespace NDSParse.Conversion.Textures.Images.Types;

public class ImageTypeBase
{
    public string Name;
    public int Width;
    public int Height;
    public IPixel[] Pixels;

    public ImageTypeBase(int width, int height, IPixel[] pixels, string name = "")
    {
        Width = width;
        Height = height;
        Pixels = pixels;
        Name = name;
    }
}