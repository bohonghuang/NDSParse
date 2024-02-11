using System.Text;
using GenericReader;
using NDSParse.Objects.Files;

namespace NDSParse.Data;

public class AssetReader : BaseReader
{
    public readonly FileBase? File;
    public override BaseReader AbsoluteOwner => this;

    public AssetReader(byte[] buffer, FileBase? file = null) : base(buffer, file?.Name ?? string.Empty)
    {
        File = file;
    }
}