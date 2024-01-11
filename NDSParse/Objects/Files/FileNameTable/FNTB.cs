using NDSParse.Data;
using NDSParse.Extensions;

namespace NDSParse.Objects.Files.FileNameTable;

public class FNTB : FNTBase
{
    private const string MAGIC = "FNTB";
    
    public FNTB(BaseReader reader)
    {
        var magic = reader.ReadString(4).Reversed();
        if (magic != MAGIC)
        {
            throw new ParserException($"{MAGIC} has invalid magic: {magic}");
        }

        var size = reader.Read<uint>();
        var directoryStartOffset = reader.Read<int>();
        var firstFilePos = reader.Read<ushort>();
        
        var numberOfFolders = reader.Read<ushort>();
        if (numberOfFolders != 1)
        {
            FirstID = LoadDirectory(reader);
        }
    }
}