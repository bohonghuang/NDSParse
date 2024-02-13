using NDSParse.Data;
using NDSParse.Objects.Files;
using Newtonsoft.Json;
using Serilog;

namespace NDSParse.Objects.Exports;

public class NDSObject : Deserializable
{
    public string Name => File?.Name ?? string.Empty;
    public string Path => File?.Path ?? string.Empty;
    public FileBase? File;

    public List<NDSExport> Blocks = [];
    
    public string ReadMagic;
    public Version Version;
    public uint FileSize;
    public ushort BlockStart;
    public ushort BlockCount;

    public virtual string Magic => string.Empty;
    public virtual bool HasBlockOffsets => false;
    
    public override void Deserialize(BaseReader reader)
    {
        if (reader is AssetReader assetReader)
        {
            File = assetReader.File;
        }
        
        ReadMagic = reader.ReadString(4);
        if (ReadMagic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {ReadMagic}");
        }

        var bom = reader.Read<ushort>();
        var version = reader.Read<ushort>();
        if (bom == 0xFFFE) version = (ushort) ((version & 0xFF) << 8 | version >> 8);
        Version = new Version(version >> 8, version & 0xFF);
        FileSize = reader.Read<uint>();
        BlockStart = reader.Read<ushort>();
        BlockCount = reader.Read<ushort>();

        if (HasBlockOffsets)
        {
            var blockOffsets = reader.ReadArray(BlockCount, reader.Read<uint>);
            foreach (var offset in blockOffsets)
            {
                reader.Position = offset;
                var extension = reader.PeekExtension();
                if (FileTypeRegistry.TryGetType(extension, out var type))
                {
                    Blocks.Add(ConstructExport<NDSExport>(reader.Spliced(), type));
                }
                else
                {
                    Log.Warning("Unknown Block {0} in File {1}", extension, File?.Path);
                }
            }
        }
        else
        {
            reader.Position = BlockStart;
            for (var blockIndex = 0; blockIndex < BlockCount; blockIndex++)
            {
                var startPosition = reader.Position;
            
                var (extension, fileSize) = reader.Peek(() => (reader.ReadString(4), reader.Read<uint>()));
                if (FileTypeRegistry.TryGetType(extension, out var type))
                {
                    Blocks.Add(ConstructExport<NDSExport>(reader.Spliced(), type));
                }
                else
                {
                    Log.Warning("Unknown Block {0} in File {1}", extension, File?.Path);
                }
            
                reader.Position = startPosition + fileSize;
            }
        }
    }

    public static NDSObject DeserializeGenericHeader(BaseReader reader)
    {
        var obj = new NDSObject();
        obj.ReadMagic = reader.ReadString(4);

        var bom = reader.Read<ushort>();
        var version = reader.Read<ushort>();
        if (bom == 0xFFFE) version = (ushort) ((version & 0xFF) << 8 | version >> 8);
        obj.Version = new Version(version >> 8, version & 0xFF);
        obj.FileSize = reader.Read<uint>();
        obj.BlockStart = reader.Read<ushort>();
        obj.BlockCount = reader.Read<ushort>();

        var subFileOffsets = reader.ReadArray<uint>( obj.BlockCount);
        foreach (var offset in subFileOffsets)
        {
            reader.Position = offset;
            var (extension, fileSize) = reader.Peek(() => (reader.ReadString(4), reader.Read<uint>()));
            if (FileTypeRegistry.TryGetType(extension, out var type))
            {
                obj.Blocks.Add(Construct<NDSExport>(reader, type));
            }
            else
            {
                reader.Position += fileSize;
            }
        }

        return obj;
    }

    public T ConstructExport<T>(BaseReader reader) where T : NDSExport, new()
    {
        var asset = Construct<T>(reader, asset => asset.Parent = this);
        return asset;
    }
    
    public T ConstructExport<T>(BaseReader reader, Action<T> dataModifier) where T : NDSExport, new()
    {
        var asset = Construct<T>(reader, asset =>
        {
            asset.Parent = this;
            dataModifier(asset);
        });
        return asset;
    }
    
    public T ConstructExport<T>(BaseReader reader, Type type) where T : NDSExport
    {
        var asset = Construct<T>(reader, type, asset => asset.Parent = this);
        return asset;
    }
    
    public T ConstructExport<T>(BaseReader reader, Type type, Action<T> dataModifier) where T : NDSExport
    {
        var asset = Construct<T>(reader, type, asset =>
        {
            asset.Parent = this;
            dataModifier(asset);
        });
        return asset;
    }

    public T GetBlock<T>() => Blocks.OfType<T>().First();
    public T[] GetBlocks<T>() => Blocks.OfType<T>().ToArray();

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}