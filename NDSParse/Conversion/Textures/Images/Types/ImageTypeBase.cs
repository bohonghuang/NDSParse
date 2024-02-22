using IPixel = NDSParse.Conversion.Textures.Pixels.IPixel;

namespace NDSParse.Conversion.Textures.Images.Types;

public class ImageTypeBase
{
    public string Name;
    public ImageMetaData MetaData;
    public IPixel[] Pixels;

    public ImageTypeBase(IPixel[] pixels, ImageMetaData metaData, string name = "")
    {
        Pixels = pixels;
        Name = name;
        MetaData = metaData;
    }
}

public class ImageMetaData
{
    public TextureFormat Format;
    public int Width;
    public int Height;
    public bool RepeatU;
    public bool RepeatV;
    public bool FlipU;
    public bool FlipV;

    public ImageMetaData(int width, int height, TextureFormat format = TextureFormat.None, bool repeatU = false, bool repeatV = false, bool flipU = false, bool flipV = false)
    {
        Format = format;
        Width = width;
        Height = height;
        RepeatU = repeatU;
        RepeatV = repeatV;
        FlipU = flipU;
        FlipV = flipV;
    }
    
}