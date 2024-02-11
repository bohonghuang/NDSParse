using NDSParse.Conversion.Textures.Colors;
namespace NDSParse.Conversion.Textures.Palettes;

public class Palette
{
    public string Name;
    public List<Color> Colors;
    public bool IsBlank => Colors.All(color => color is { R: 0, G: 0, B: 0 });

    public Palette(IEnumerable<Color>? colors = null, string name = "")
    {
        Colors = colors is not null ? [..colors] : [];
        Name = name;
    }
    
}