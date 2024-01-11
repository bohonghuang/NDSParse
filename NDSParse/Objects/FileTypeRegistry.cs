using System.Reflection;
using NDSParse.Objects.Exports;

namespace NDSParse.Objects;

public static class FileTypeRegistry
{
    private static readonly List<string> FileTypes = [];
    private static readonly Type DefaultObjectType = typeof(NDSObject);
    
    static FileTypeRegistry()
    {
        Register<NDSObject>();
    }

    public static void Register<T>()
    {
        var assembly = typeof(T).Assembly;
        foreach (var type in assembly.DefinedTypes)
        {
            if (type.IsAbstract || type.IsInterface || !DefaultObjectType.IsAssignableFrom(type) || DefaultObjectType == type)
            {
                continue;
            }
            
            var obj = (NDSObject) Activator.CreateInstance(type)!;
            FileTypes.Add(obj.Magic);
        }
    }

    public static bool Contains(string str)
    {
        return FileTypes.Any(type => type.Equals(str, StringComparison.OrdinalIgnoreCase));
    }
}