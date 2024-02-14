using NDSParse.Conversion.Textures.Colors;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Data;
using Serilog;

namespace NDSParse.Objects.Exports.Fonts;

public class PLGC : NDSExport
{
    public byte BoxWidth;
    public byte BoxHeight;
    public ushort BoxByteSize;
    public byte CharacterHeight;
    public byte CharacterWidth;
    public byte Depth;
    public byte Rotation;
    public int CharacterCount;
    public Palette Palette;

    public List<WidthInfo> EmbeddedWidthInfos = [];

    public List<byte[]> Tiles = [];
    
    public override string Magic => "PLGC";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        BoxWidth = reader.ReadByte();
        BoxHeight = reader.ReadByte();
        BoxByteSize = reader.Read<ushort>();
        CharacterHeight = reader.ReadByte();
        CharacterWidth = reader.ReadByte();
        Depth = reader.ReadByte();
        Rotation = reader.ReadByte();

        CharacterCount = (int) ((DataSize - 8) / BoxByteSize);
        for (var i = 0; i < CharacterCount; i++)
        {
            if (Parent.File.Provider.Header.GameCode.Equals("IRBO") || Parent.File.Provider.Header.GameCode.Equals("IREO"))
            {
                EmbeddedWidthInfos.Add(Construct<WidthInfo>(reader));
                Tiles.Add(BytesToBits(reader.ReadBytes(BoxByteSize - WidthInfo.SIZE)));
            }
            else
            {
                Tiles.Add(BytesToBits(reader.ReadBytes(BoxByteSize)));
            }
        }

        Palette = CalculatePalette(Depth);
    }
    
    private Palette CalculatePalette(int depth)
    {
        var palette = new Palette();

        var colorCount = 1 << depth;
        for (var i = 0; i < colorCount; i++)
        {
            var colorIndex = (byte) (i * (255 / (colorCount - 1)));
            palette.Colors.Add(new Color(colorIndex, colorIndex, colorIndex, (byte) (i == 0 ? 0 : 255)));
        }

        return palette;
    }
    
    private byte[] BytesToBits(IEnumerable<byte> bytes)
    {
        var bits = new List<byte>();

        foreach (var byteData in bytes)
            for (var j = 7; j >= 0; j--)
                bits.Add((byte)((byteData >> j) & 1));

        return bits.ToArray();
    }
}
