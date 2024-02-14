using NDSParse.Data;

namespace NDSParse.Objects.Files;

public class FileBase
{
    public string Path;
    public string Name => Path.Split("/").Last();
    public string Type => Path.Split(".").Last();
    public NDSProvider Provider;
    internal DataBlock Data;

    public FileBase(string path)
    {
        Path = path;
    }
    
    public AssetReader CreateReader() => Data.CreateAssetReader(this);

    public GameFile Copy(string? path = null) => new(path ?? Path, Data)
    {
        Provider = Provider
    };
}