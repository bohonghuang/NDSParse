using NDSParse.Data;

namespace NDSParse.Objects.Exports.Textures.Cell;

public class RECN : NDSObject
{
    public KBEC CellBank;
    public LBAL Labels;
    
    public override string Magic => "RECN";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        CellBank = GetBlock<KBEC>();
        Labels = GetBlock<LBAL>();
    }
}