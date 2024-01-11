using System.Text;
using GenericReader;

namespace NDSParse.Data;

public class AssetReader : BaseReader
{
    public override BaseReader AbsoluteOwner => this;

    public AssetReader(byte[] buffer, string name = "") : base(buffer, name) { }
}