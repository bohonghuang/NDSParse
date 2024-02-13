using NDSParse.Data;
using NDSParse.Extensions;

namespace NDSParse.Objects.Exports.Archive;

public class GMIF : NDSExport
{
    public DataBlock Data;
    
    public override string Magic => "GMIF";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        Data = new DataBlock(reader, (int) reader.Position, (int) DataSize);
    }
}