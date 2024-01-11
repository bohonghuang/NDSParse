using NDSParse.Conversion.Textures.Pixels;

namespace NDSParse.Conversion.Textures.Images.Types;

public class IndexedImage : ImageTypeBase
{
    public IndexedImage(int width, int height, IndexedPixel[] pixels) : base(width, height, pixels)
    {
        
    }
}