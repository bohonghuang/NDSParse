using NDSParse.Conversion.Textures;
using NDSParse.Conversion.Textures.Colors;
using NDSParse.Conversion.Textures.Colors.Types;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Data;

namespace NDSParse.Objects.Exports.Palettes;

public class TTLP : NDSExport
{
    public List<Palette> Palettes = [];
    public ColorFormat Format;
    public uint PaletteSize;
    public uint ColorsPerPalette;
    
    public override string Magic => "TTLP";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        Format = reader.ReadEnum<ColorFormat, ushort>();
        
        reader.Position += 6;
        
        PaletteSize = reader.Read<uint>();
        ColorsPerPalette = reader.Read<uint>();

        var paletteCount = PaletteSize / (ColorsPerPalette * 2);
        for (var paletteIndex = 0; paletteIndex < paletteCount; paletteIndex++)
        {
            var colors = reader.ReadColors<BGR555>((int) ColorsPerPalette);
            Palettes.Add(new Palette(colors, $"{Parent.Name}_{paletteIndex}"));
        }
    }
}

public enum ColorFormat : ushort
{
    Color16 = 3,
    Color256 = 4,
}