using System.Reflection;
using NDSParse.Objects.Exports;

namespace NDSParse.Objects;

public static class FileTypeRegistry
{
    private static readonly Dictionary<string, Type> Types = [];
    private static readonly Type DefaultObjectType = typeof(NDSObject);
    private static readonly Type DefaultExportType = typeof(NDSExport);
    
    static FileTypeRegistry()
    {
        Register<NDSObject>();
    }

    public static void Register<T>()
    {
        var assembly = typeof(T).Assembly;
        foreach (var type in assembly.DefinedTypes)
        {
            if (type.IsAbstract || type.IsInterface) continue;
            if (DefaultObjectType == type || DefaultExportType == type) continue;
            try
            {
                if (DefaultObjectType.IsAssignableFrom(type))
                {
                    var obj = (NDSObject) Activator.CreateInstance(type)!;
                    if (!string.IsNullOrEmpty(obj.Magic))
                        Types[obj.Magic] = type;
                }

                if (DefaultExportType.IsAssignableFrom(type))
                {
                    var export = (NDSExport) Activator.CreateInstance(type)!;
                    if (!string.IsNullOrEmpty(export.Magic))
                        Types[export.Magic] = type;
                }
            }
            catch (Exception e)
            {
                // ignored
            }

        }
    }

    public static bool Contains(string str)
    {
        return Types.Keys.Any(type => type.Equals(str, StringComparison.OrdinalIgnoreCase));
    }
    
    public static bool TryGetType(string str, out Type type)
    {
        return Types.TryGetValue(str, out type);
    }
}