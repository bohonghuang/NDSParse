using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Indexed;

namespace NDSParse.Conversion.Textures.Images.Types;

public class IndexedPaletteImage : IndexedImage
{
    public bool IsFirstColorTransparent;
    public List<Palette> Palettes;
    
    public IndexedPaletteImage(int width, int height, IPixel[] pixels, List<Palette> palettes, string name = "", bool isFirstColorTransparent = false) : base(width, height, pixels, name)
    {
        Palettes = palettes;
        IsFirstColorTransparent = isFirstColorTransparent;
    }
    
    public IndexedPaletteImage(int width, int height, IPixel[] pixels, string name = "") : base(width, height, pixels, name)
    {
        Palettes = new List<Palette>();
        IsFirstColorTransparent = false;
    }
}