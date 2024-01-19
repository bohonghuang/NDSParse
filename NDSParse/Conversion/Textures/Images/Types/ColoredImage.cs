using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Indexed;

namespace NDSParse.Conversion.Textures.Images.Types;

public class ColoredImage : ImageTypeBase
{
    public ColoredImage(IPixel[] pixels, ImageMetaData metaData, string name = "") : base(pixels, metaData, name)
    {
        
    }
}