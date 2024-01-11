using NDSParse.Data;

namespace NDSParse.Objects.Files.FileAllocationTable;

public class FAT : FATBase
{
    public FAT(BaseReader reader)
    {
        var fileCount = reader.Size / (sizeof(int) * 2);
        for (var i = 0; i < fileCount; i++)
        {
            FileBlocks.Add(new DataBlock(reader, DataBlockReadType.StartEnd));
        }
    }
}