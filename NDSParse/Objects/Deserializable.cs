using NDSParse.Data;

namespace NDSParse.Objects;

public abstract class Deserializable
{
    public abstract void Deserialize(BaseReader reader);
    
    public static T Construct<T>(BaseReader reader) where T : Deserializable, new()
    {
        var ret = new T();
        ret.Deserialize(reader);
        return ret;
    }
}

public abstract class NamedDeserializable : Deserializable
{
    public string Name;
}