using System.Collections;
using NDSParse.Data;

namespace NDSParse.Objects.Exports;

public class NameList<T> : NameListBase<T> where T : Deserializable, new()
{
    public NameList(BaseReader reader, Func<T> func) : base(reader, func) { }
    
    public NameList(BaseReader reader) : base(reader, () => Deserializable.Construct<T>(reader)) { }
}

public class NameListUnmanaged<T> : NameListBase<T> where T : unmanaged
{
    public NameListUnmanaged(BaseReader reader, Func<T> func) : base(reader, func) { }
    
    public NameListUnmanaged(BaseReader reader) : base(reader, reader.Read<T>) { }
}

public abstract class NameListBase<T> : IEnumerable
{
    public string[] Names => Dict.Keys.ToArray();
    public T[] Values => Dict.Values.ToArray();
    public int Count => Dict.Count;
    
    public readonly Dictionary<string, T> Dict = new();

    public KeyValuePair<string, T> Get(int i) => Dict.ElementAt(i >= Dict.Count ? 0 : i);

    protected NameListBase(BaseReader reader, Func<T> func)
    {
        var dummy = reader.ReadByte();
        var count = reader.ReadByte();
        var size = reader.Read<ushort>();

        var subheaderSize = reader.Read<ushort>();
        var unknownSize = reader.Read<ushort>();
        var unknown = reader.Read<uint>();
        var unknownArray = reader.ReadArray<uint>(count);

        var elementSize = reader.Read<ushort>();
        var dataSize = reader.Read<ushort>();

        var data = reader.ReadArray(count, func);
        var names = reader.ReadArray(count, () => reader.ReadString(16));

        for (var i = 0; i < count; i++)
        {
            Dict[names[i]] = data[i];
        }
    }
    
    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return Dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public string GetKeyFromValue(T input)
    {
        var foundPair = Dict.FirstOrDefault(pair => pair.Value.Equals(input));
        return foundPair.Key;
    }
}
