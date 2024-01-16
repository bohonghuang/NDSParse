using NDSParse.Data;
using NDSParse.Objects.Files;
using Newtonsoft.Json;

namespace NDSParse.Objects.Exports;

public class NDSObject : Deserializable
{
    public string Name => File?.Name ?? string.Empty;
    public string Path => File?.Path ?? string.Empty;
    public FileBase? File;
    
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
        
        var magic = reader.ReadString(4);
        if (magic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic: {magic}");
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