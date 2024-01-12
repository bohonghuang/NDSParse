using NDSParse.Data;
using Newtonsoft.Json;

namespace NDSParse.Objects.Exports;

public class NDSExport : Deserializable
{
    public uint FileSize;

    public virtual string Magic => string.Empty;

    public const int HEADER_SIZE = 8;

    private long Offset;
    private long Size;
    
    public override void Deserialize(BaseReader reader)
    {
        var magic = reader.ReadString(4).Trim();
        if (magic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {magic}");
        }

        FileSize = reader.Read<uint>();
        Offset = reader.Position;
        Size = FileSize - HEADER_SIZE;
    }

    protected BaseReader CreateDataReader(BaseReader reader)
    {
        return reader.Spliced((uint) Offset, (uint) Size);
    }

    protected DataBlock CreateDataBlock(BaseReader reader)
    {
        return new DataBlock(reader, (int) Offset, (int) Size);
    }
    
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}