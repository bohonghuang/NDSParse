namespace NDSParse.Conversion.Textures.Pixels;

public class IndexedPixel
{
    public short Index;
    public byte Alpha;
    public byte PaletteIndex;
    
    public IndexedPixel(short index)
    {
        Index = index;
        Alpha = 255;
        PaletteIndex = 0;
    }
}