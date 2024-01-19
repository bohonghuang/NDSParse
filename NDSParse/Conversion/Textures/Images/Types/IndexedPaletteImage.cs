using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Indexed;

namespace NDSParse.Conversion.Textures.Images.Types;

public class IndexedPaletteImage : IndexedImage
{
    public bool IsFirstColorTransparent;
    public List<Palette> Palettes;
    
    public IndexedPaletteImage(IPixel[] pixels, List<Palette> palettes, ImageMetaData metaData, string name = "", bool isFirstColorTransparent = false) : base(pixels, metaData, name)
    {
        Palettes = palettes;
        IsFirstColorTransparent = isFirstColorTransparent;
    }
    
    public IndexedPaletteImage(IPixel[] pixels, ImageMetaData metaData, string name = "") : base(pixels, metaData, name)
    {
        Palettes = new List<Palette>();
        IsFirstColorTransparent = false;
    }
}