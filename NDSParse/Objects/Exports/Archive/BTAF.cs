using NDSParse.Data;
using NDSParse.Extensions;
using NDSParse.Objects.Files.FileAllocationTable;

namespace NDSParse.Objects.Exports.Archive;

public class BTAF : FATBase
{
    public override string Magic => "BTAF";
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        var fileCount = reader.Read<int>();
        for (var i = 0; i < fileCount; i++)
        {
            FileBlocks.Add(new DataBlock(reader, DataBlockReadType.StartEnd));
        }
    }

}