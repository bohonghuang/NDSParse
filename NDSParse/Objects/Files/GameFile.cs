using NDSParse.Data;

namespace NDSParse.Objects.Files;

public class GameFile
{
    public string Name;
    public string Path;
    public string? Type;

    internal DataBlock Data;

    public GameFile(string path, DataBlock data)
    {
        Path = path;
        Name = path.Split("/").Last();
        Type = path.Contains('.') ? path.Split(".").Last() : default;
        Data = data;
    }
    
    public AssetReader CreateReader() => Data.CreateAssetReader();
    
}