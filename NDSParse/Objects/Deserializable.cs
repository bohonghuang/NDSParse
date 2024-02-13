using NDSParse.Data;

namespace NDSParse.Objects;

public abstract class Deserializable
{
    public abstract void Deserialize(BaseReader reader);
    
    public static T Construct<T>(BaseReader reader) where T : Deserializable
    {
        var ret = Activator.CreateInstance<T>();
        ret.Deserialize(reader);
        return ret;
    }
    
    public static T Construct<T>(BaseReader reader, Action<T> dataModifier) where T : Deserializable
    {
        var ret = Activator.CreateInstance<T>();
        dataModifier.Invoke(ret);
        ret.Deserialize(reader);
        return ret;
    }
    
    public static T Construct<T>(BaseReader reader, Type type) where T : Deserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        ret!.Deserialize(reader);
        return ret;
    }
    
    public static T Construct<T>(BaseReader reader, Type type, Action<T> dataModifier) where T : Deserializable
    {
        var ret = Activator.CreateInstance(type) as T;
        dataModifier.Invoke(ret);
        ret.Deserialize(reader);
        return ret;
    }
}

public abstract class NamedDeserializable : Deserializable
{
    public string Name;
}