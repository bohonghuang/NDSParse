using NDSParse.Data;
using Serilog;

namespace NDSParse.Objects.Exports.Fonts;

public class HDWC : NDSExport
{
    public bool IsValid = true;
    public ushort FirstChar;
    public ushort LastChar;

    public List<WidthInfo> Infos = [];
    
    public override string Magic => "HDWC";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        // todo fix ugly
        if (Parent.File.Provider.Header.GameCode.Equals("IRBO") || Parent.File.Provider.Header.GameCode.Equals("IREO"))
        {
            IsValid = false;
            return;
        }
        
        FirstChar = reader.Read<ushort>();
        LastChar = reader.Read<ushort>();
        reader.Position += sizeof(uint);

        for (var i = FirstChar; i <= LastChar; i++)
        {
            Infos.Add(Construct<WidthInfo>(reader));
        }
    }
}

public class WidthInfo : Deserializable
{
    public sbyte Left;
    public byte GlyphWidth;
    public byte CharacterWidth;

    public const int SIZE = sizeof(sbyte) + sizeof(byte) * 2;
    public override void Deserialize(BaseReader reader)
    {
        Left = reader.Read<sbyte>();
        GlyphWidth = reader.ReadByte();
        CharacterWidth = reader.ReadByte();
    }
}