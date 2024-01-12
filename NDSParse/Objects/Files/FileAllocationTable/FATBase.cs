using NDSParse.Data;
using NDSParse.Objects.Exports;

namespace NDSParse.Objects.Files.FileAllocationTable;

public class FATBase : NDSExport
{
    public List<DataBlock> FileBlocks = [];
}