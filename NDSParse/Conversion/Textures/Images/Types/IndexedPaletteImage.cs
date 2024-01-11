using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;

namespace NDSParse.Conversion.Textures.Images.Types;

public class IndexedPaletteImage : ImageTypeBase
{
    public List<Palette> Palettes;
    
    public IndexedPaletteImage(int width, int height, IndexedPixel[] pixels, List<Palette> palettes) : base(width, height, pixels)
    {
        Palettes = palettes;
    }
    
    public IndexedPaletteImage(int width, int height, IndexedPixel[] pixels) : base(width, height, pixels)
    {
        Palettes = new List<Palette>();
    }
}