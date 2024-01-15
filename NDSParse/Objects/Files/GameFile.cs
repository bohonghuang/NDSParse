using NDSParse.Data;

namespace NDSParse.Objects.Files;

public class GameFile : FileBase
{
    internal DataBlock Data;

    public GameFile(string path, DataBlock data) : base(path)
    {
        Data = data;
    }
    
    public AssetReader CreateReader() => Data.CreateAssetReader(this);

    public GameFile Copy(string? path = null) => new(path ?? Path, Data);

}