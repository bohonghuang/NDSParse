using NDSParse.Data;
using NDSParse.Objects.Files.FileNameTable;

namespace NDSParse.Objects.Exports.Archive;

public class BTNF : FNTBase
{
    public override string Magic => "BTNF";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        LoadDirectory(CreateDataReader(reader));
    }
}