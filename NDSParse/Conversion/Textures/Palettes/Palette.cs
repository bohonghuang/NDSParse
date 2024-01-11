using NDSParse.Conversion.Textures.Colors;
using SixLabors.ImageSharp.PixelFormats;
namespace NDSParse.Conversion.Textures.Palettes;

public class Palette
{
    public string Name;
    public List<Color> Colors;
    public bool IsBlank => Colors.All(color => color is { R: 0, G: 0, B: 0 });

    public Palette(IEnumerable<Color> colors, string name = "")
    {
        Colors = new List<Color>(colors);
        Name = name;
    }
}