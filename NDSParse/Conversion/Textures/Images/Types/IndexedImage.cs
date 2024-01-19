using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Indexed;

namespace NDSParse.Conversion.Textures.Images.Types;

public class IndexedImage : ImageTypeBase
{
    public IndexedImage(IPixel[] pixels, ImageMetaData metaData, string name = "") : base(pixels, metaData, name)
    {
        
    }
}