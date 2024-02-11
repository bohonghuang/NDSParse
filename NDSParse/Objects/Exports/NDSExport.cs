using NDSParse.Data;
using Newtonsoft.Json;

namespace NDSParse.Objects.Exports;

// todo refactor to get auto loaded by all nds object
public class NDSExport : Deserializable
{
    public uint FileSize;
    public long DataSize;
    public NDSObject Parent;

    public virtual string Magic => string.Empty;

    public const int HEADER_SIZE = 8;

    private long Offset;
    
    public override void Deserialize(BaseReader reader)
    {
        var magic = reader.ReadString(4).Trim();
        if (magic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {magic}");
        }

        FileSize = reader.Read<uint>();
        Offset = reader.Position;
        DataSize = FileSize - HEADER_SIZE;
    }

    protected BaseReader CreateDataReader(BaseReader reader)
    {
        return reader.Spliced((uint) Offset, (uint) DataSize);
    }

    protected DataBlock CreateDataBlock(BaseReader reader)
    {
        return new DataBlock(reader, (int) Offset, (int) DataSize);
    }
    
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}