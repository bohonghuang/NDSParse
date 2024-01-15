using System.Text;
using GenericReader;
using NDSParse.Objects.Files;

namespace NDSParse.Data;

public class AssetReader : BaseReader
{
    public readonly GameFile? File;
    public override BaseReader AbsoluteOwner => this;

    public AssetReader(byte[] buffer, GameFile? file = null) : base(buffer, file?.Name ?? string.Empty)
    {
        File = file;
    }
}