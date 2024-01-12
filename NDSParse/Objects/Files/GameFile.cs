using NDSParse.Data;

namespace NDSParse.Objects.Files;

public class GameFile
{
    public string Path;
    public string Name => Path.Split("/").Last();
    public string Type => Path.Split(".").Last();

    internal DataBlock Data;

    public GameFile(string path, DataBlock data)
    {
        Path = path;
        Data = data;
    }
    
    public AssetReader CreateReader() => Data.CreateAssetReader(this);

    public GameFile Copy() => new(Path, Data);

}