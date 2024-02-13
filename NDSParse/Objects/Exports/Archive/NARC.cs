using NDSParse.Data;
using NDSParse.Objects.Files;

namespace NDSParse.Objects.Exports.Archive;

public class NARC : NDSObject
{
    public readonly Dictionary<string, GameFile> Files = new();
    
    public override string Magic => "NARC";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        var fat = GetBlock<BTAF>();
        var fnt = GetBlock<BTNF>();
        var gmif = GetBlock<GMIF>();
        
        // technicaly gmif data but i would rather control it out here
        var dataReader = gmif.Data.CreateSealedReader();
        for (ushort id = 0; id < fat.FileBlocks.Count; id++)
        {
            var fileBlock = fat.FileBlocks[id];
            var startPosition = fileBlock.Offset;

            string name;
            if (id < fnt.FilesById.Count)
            {
                name = fnt.FilesById[id];
            }
            else if (fileBlock.Length > 0)
            {
                dataReader.Position = startPosition;
                var extension = dataReader.PeekExtension();
                if (!FileTypeRegistry.Contains(extension)) extension = "bin";
                
                name = $"{id}.{extension}";
            }
            else
            {
                continue;
            }

            Files[name] = new GameFile(name, new DataBlock(dataReader, startPosition, fileBlock.Length));
        }
    }

    public void MountToProvider(NDSProvider provider)
    {
        var basePath = Path.Replace(".narc", string.Empty);
        foreach (var (path, gameFile) in Files)
        {
            var newPath = basePath + $"/{path}";
            provider.Files[newPath] = gameFile.Copy(newPath);
        }
    }
}