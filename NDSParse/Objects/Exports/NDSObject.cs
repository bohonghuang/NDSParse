using NDSParse.Data;
using NDSParse.Objects.Files;
using Newtonsoft.Json;

namespace NDSParse.Objects.Exports;

public class NDSObject : Deserializable
{
    public string Name => File?.Name ?? string.Empty;
    public string Path => File?.Path ?? string.Empty;
    public FileBase? File;
    
    public string ReadMagic;
    public ushort Version;
    public uint FileSize;
    public ushort HeaderSize;
    public ushort SubFileCount;
    public uint[] SubFileOffsets;

    public virtual string Magic => string.Empty;
    protected virtual bool ContainsSubfiles => false;
    
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
        Version = reader.Read<ushort>();
        if (bom == 0xFFFE) Version = (ushort) ((Version & 0xFF) << 8 | Version >> 8);
        FileSize = reader.Read<uint>();
        HeaderSize = reader.Read<ushort>();
        SubFileCount = reader.Read<ushort>();

        if (ContainsSubfiles)
        {
            SubFileOffsets = reader.ReadArray<uint>(SubFileCount);
        }
    }

    public static NDSObject DeserializeGenericHeader(BaseReader reader, bool hasSubfiles = false)
    {
        var obj = new NDSObject();
        obj.ReadMagic = reader.ReadString(4);

        var bom = reader.Read<ushort>();
        obj.Version = reader.Read<ushort>();
        if (bom == 0xFFFE) obj.Version = (ushort) ((obj.Version & 0xFF) << 8 | obj.Version >> 8);
        obj.FileSize = reader.Read<uint>();
        obj.HeaderSize = reader.Read<ushort>();
        obj.SubFileCount = reader.Read<ushort>();

        if (hasSubfiles)
        {
            obj.SubFileOffsets = reader.ReadArray<uint>(obj.SubFileCount);
        }

        return obj;
    }

    public void ManageSubFiles(BaseReader reader, Action<string, uint> extensionHandler)
    {
        foreach (var offset in SubFileOffsets)
        {
            reader.Position = offset;
            var extension = reader.Peek(() => reader.ReadString(4));
            extensionHandler(extension, offset);
        }
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}