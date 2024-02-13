using System.Text;
using GenericReader;
using NDSParse.Objects.Files;

namespace NDSParse.Data;

public class AssetReader : SealedReader
{
    public readonly FileBase? File;

    public AssetReader(byte[] buffer, FileBase? file = null) : base(buffer, file?.Name ?? string.Empty)
    {
        File = file;
    }
}