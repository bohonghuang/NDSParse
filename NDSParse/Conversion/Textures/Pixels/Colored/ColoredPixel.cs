using NDSParse.Conversion.Textures.Colors;

namespace NDSParse.Conversion.Textures.Pixels.Colored;

public class ColoredPixel : IPixel
{
    public Color Color;

    public ColoredPixel(Color color)
    {
        Color = color;
    }
}