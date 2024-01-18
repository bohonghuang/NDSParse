using NDSParse.Data;
using NDSParse.Objects;

namespace NDSParse.Games.Pokemon.BW2.Map;

public class BW2MapHeader : Deserializable
{
    public List<MapHeader> Headers = [];
    
    private const int DataSize = 48;
    
    public override void Deserialize(BaseReader reader)
    {
        var headerCount = reader.Size / DataSize;

        for (var headerIndex = 0; headerIndex < headerCount; headerIndex++)
        {
            Headers.Add(Construct<MapHeader>(reader));
        }
    }

    public void AssignNames(BW2Texts bw2Texts)
    {
        foreach (var header in Headers)
        {
            header.Name = bw2Texts.TextList[header.NameIndex];
        }
    }
}

public class MapHeader : Deserializable
{
    public string Name;
    
    public byte MapType;
    // unknown BYTE
    public ushort TextureIndex;
    public ushort MatrixIndex;
    public ushort ScriptIndex;
    public ushort LevelScriptIndex;
    public ushort TextIndex;
    public ushort SpringMusicIndex;
    public ushort SummerMusicIndex;
    public ushort AutumnMusicIndex;
    public ushort WinterMusicIndex;
    // unknown BYTE * 2
    public ushort MapID;
    public ushort ParentMapID;
    public byte NameIndex;
    // sizeof(byte) * 5 + sizeof(ushort) + sizeof(byte) * 2 + sizeof(uint) * 3
    public override void Deserialize(BaseReader reader)
    {
        MapType = reader.ReadByte();
        reader.Position += 1; // unknown
        TextureIndex = reader.Read<ushort>();
        MatrixIndex = reader.Read<ushort>();
        ScriptIndex = reader.Read<ushort>();
        LevelScriptIndex = reader.Read<ushort>();
        TextIndex = reader.Read<ushort>();
        SpringMusicIndex = reader.Read<ushort>();
        SummerMusicIndex = reader.Read<ushort>();
        AutumnMusicIndex = reader.Read<ushort>();
        WinterMusicIndex = reader.Read<ushort>();
        reader.Position += 2; // unknown
        MapID = reader.Read<ushort>();
        ParentMapID = reader.Read<ushort>();
        NameIndex = reader.ReadByte();
        reader.Position += 5;
        reader.Position += sizeof(ushort);
        reader.Position += 2;
        reader.Position += sizeof(uint) * 3;

    }
}