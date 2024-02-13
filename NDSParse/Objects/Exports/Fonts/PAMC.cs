using NDSParse.Data;
using Serilog;

namespace NDSParse.Objects.Exports.Fonts;

public class PAMC : NDSExport
{
    public ushort FirstChar;
    public ushort LastChar;
    public uint Type;
    public uint NextMapOffset;

    public readonly Dictionary<int, ushort> Map = [];
    
    public override string Magic => "PAMC";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        FirstChar = reader.Read<ushort>();
        LastChar = reader.Read<ushort>();
        Type = reader.Read<uint>();
        NextMapOffset = reader.Read<uint>();

        var mapEntries = LastChar - FirstChar + 1;
        switch (Type)
        {
            case 0:
            {
                var firstCharacter = reader.Read<ushort>();
                for (ushort i = 0; i < mapEntries; i++)
                {
                    Map[firstCharacter + i] = (ushort) (FirstChar + i);
                }
                break;
            }
            case 1:
            {
                for (ushort i = 0; i < mapEntries; i++)
                {
                    var characterIndex = reader.Read<ushort>();
                    if (characterIndex != 0xFFFF)
                        Map[characterIndex] = (ushort) (FirstChar + i);
                }
                break;
            }
            case 2:
            {
                mapEntries = reader.Read<ushort>();
                for (ushort i = 0; i < mapEntries; i++)
                {
                    var charIndex = reader.Read<ushort>();
                    var mapIndex = reader.Read<ushort>();
                    Map[mapIndex] = charIndex;
                }
                break;
            }
            default:
            {
                throw new NotImplementedException();
            }
        }
    }
    
}

public class PAMCInfo : Deserializable
{
    public override void Deserialize(BaseReader reader)
    {
        
    }
}