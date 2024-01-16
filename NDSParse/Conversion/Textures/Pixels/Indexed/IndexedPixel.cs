namespace NDSParse.Conversion.Textures.Pixels.Indexed;

public class IndexedPixel : IPixel
{
    public ushort Index;
    public byte Alpha;
    public byte PaletteIndex;

    public IndexedPixel(ushort index = 0, byte alpha = 255, byte paletteIndex = 0)
    {
        Index = index;
        Alpha = alpha;
        PaletteIndex = paletteIndex;
    }
}