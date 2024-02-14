using NDSParse.Data;

namespace NDSParse.Objects.Exports.Palettes;

public class RLCN : NDSObject
{
    public TTLP PaletteData;
    // do we even need PMCP
    // might be important for palette indices referenced by other files but whatever
    
    public override string Magic => "RLCN";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        PaletteData = GetBlock<TTLP>();
    }
}