using NDSParse.Data;
using NDSParse.Extensions;

namespace NDSParse.Objects.Files.FileAllocationTable;

public class FATB : FATBase
{
    private const string MAGIC = "FATB";
    
    public FATB(BaseReader reader)
    {
        var magic = reader.ReadString(4).Reversed();
        if (magic != MAGIC)
        {
            throw new ParserException($"{MAGIC} has invalid magic: {magic}");
        }

        var size = reader.Read<uint>();
        var fileCount = reader.Read<int>();
        
        for (var i = 0; i < fileCount; i++)
        {
            FileBlocks.Add(new DataBlock(reader, DataBlockReadType.StartEnd));
        }
    }
}