using NDSParse.Data;
using NDSParse.Objects.Exports;

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
    

    public T Load<T>() where T : Deserializable, new() => Deserializable.Construct<T>(CreateReader());
    
    public bool TryLoad<T>(out T data) where T : Deserializable, new()
    {
        data = null!;
        try
        {
            data = Load<T>();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

}