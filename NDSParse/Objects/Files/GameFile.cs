using NDSParse.Data;
using NDSParse.Objects.Exports;

namespace NDSParse.Objects.Files;

public class GameFile : FileBase
{
    public GameFile(string path, DataBlock data) : base(path)
    {
        Data = data;
    }
    
    

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