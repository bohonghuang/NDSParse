using NDSParse.Data;
using NDSParse.Extensions;

namespace NDSParse.Objects.Exports.Archive;

public class GMIF : NDSExport
{
    public override string Magic => "GMIF";
}