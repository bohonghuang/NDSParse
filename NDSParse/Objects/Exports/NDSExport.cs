using NDSParse.Data;
using Newtonsoft.Json;

namespace NDSParse.Objects.Exports;

public class NDSExport : Deserializable
{
    public uint FileSize;

    public virtual string Magic => string.Empty;

    public const int HEADER_SIZE = 8;

    private long DataOffset;
    
    public override void Deserialize(BaseReader reader)
    {
        var magic = reader.ReadString(4).Trim();
        if (magic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {magic}");
        }

        FileSize = reader.Read<uint>();
        DataOffset = reader.Position;
    }

    public BaseReader CreateDataReader(BaseReader reader)
    {
        return reader.Spliced((uint) DataOffset, FileSize - HEADER_SIZE);
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}