using NDSParse.Data;

namespace NDSParse.Objects.Files.FileNameTable;

public class FNT : FNTBase
{
    public FNT(BaseReader reader)
    {
        FirstID = LoadDirectory(reader);
    }
}