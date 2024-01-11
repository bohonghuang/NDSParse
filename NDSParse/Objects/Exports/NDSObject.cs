using NDSParse.Data;
using Newtonsoft.Json;

namespace NDSParse.Objects.Exports;

public class NDSObject
{
    public ushort Version;
    public uint FileSize;
    public ushort HeaderSize;
    public ushort SubFileCount;
    public uint[] SubFileOffsets;

    public virtual string Magic => string.Empty;
    protected virtual bool ContainsSubfiles => false;

    public static T Construct<T>(BaseReader reader) where T : NDSObject, new()
    {
        var obj = new T();
        obj.Deserialize(reader);
        return obj;
    }
    
    public virtual void Deserialize(BaseReader reader)
    {
        var magic = reader.ReadString(4);
        if (magic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {magic}");
        }

        var bom = reader.Read<ushort>();
        Version = reader.Read<ushort>();
        if (bom == 0xFFFE) Version = (ushort) ((Version & 0xFF) << 8 | Version >> 8);
        FileSize = reader.Read<uint>();
        HeaderSize = reader.Read<ushort>();
        SubFileCount = reader.Read<ushort>();

        if (ContainsSubfiles)
        {
            SubFileOffsets = reader.ReadArray<uint>(SubFileCount);
        }
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}