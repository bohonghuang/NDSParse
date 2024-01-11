using NDSParse.Data;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Textures.Colors.Types;

public abstract class ColorTypeBase
{
    public Color[] Decode(BaseReader reader, int colorCount)
    {
        var colors = new Color[colorCount];

        for (var colorIndex = 0; colorIndex < colorCount; colorIndex++)
        {
            colors[colorIndex] = Read(reader);
        }

        return colors;
    }
    
    public abstract Color Read(BaseReader reader);
}