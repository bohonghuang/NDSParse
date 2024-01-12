using NDSParse.Data;
using NDSParse.Objects.Files;

namespace NDSParse.Objects.Exports.Archive;

public class NARC : NDSObject
{
    public Dictionary<string, GameFile> Files = new();
    
    public override string Magic => "NARC";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        var fat = Construct<BTAF>(reader);
        var fnt = Construct<BTNF>(reader);
        var gimf = Construct<GMIF>(reader);
        
        // technicaly gmif data but i would rather control it out here
        var dataOffset = (int) reader.Position;
        for (ushort id = 0; id < fat.FileBlocks.Count; id++)
        {
            var fileBlock = fat.FileBlocks[id];
            var startPosition = fileBlock.Offset + dataOffset;

            string name;
            if (id < fnt.FilesById.Count)
            {
                name = fnt.FilesById[id];
            }
            else if (fileBlock.Length > 0)
            {
                reader.Position = startPosition;
                var extension = reader.Peek(() => reader.ReadString(4)).ToLower();
                if (!FileTypeRegistry.Contains(extension)) extension = "bin";
                
                name = $"{id}.{extension}";
            }
            else
            {
                continue;
            }

            Files[name] = new GameFile(name, new DataBlock(reader, startPosition, fileBlock.Length));
        }
    }
}