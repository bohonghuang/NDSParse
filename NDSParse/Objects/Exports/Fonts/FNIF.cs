using NDSParse.Data;
using Serilog;

namespace NDSParse.Objects.Exports.Fonts;

public class FNIF : NDSExport
{
    public byte Height;
    public byte Width;
    public ushort NullCharIndex;
    public FontEncoding Encoding;

    public byte CharacterHeight;
    public byte CharacterWidth;
    public byte BearingX;
    public byte BearingY;
    
    public uint PLGCOffset;
    public uint HDWCOffset;
    public uint PAMCOffset;
    
    public override string Magic => "FNIF";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        reader.Position += 1;
        
        Height = reader.ReadByte();
        NullCharIndex = reader.Read<ushort>();
        
        reader.Position += 2;
        
        Width = reader.ReadByte();
        Encoding = reader.ReadEnum<FontEncoding, byte>();

        PLGCOffset = reader.Read<uint>();
        HDWCOffset = reader.Read<uint>();
        PAMCOffset = reader.Read<uint>();

        if (Parent.Version >= new Version(1, 2))
        {
            CharacterHeight = reader.ReadByte();
            CharacterWidth = reader.ReadByte();
            BearingY = reader.ReadByte();
            BearingX = reader.ReadByte();
        }
    }
}

public enum FontEncoding : byte
{
    UTF8,
    UTF16,
    ShiftJIS,
    CP1252
}