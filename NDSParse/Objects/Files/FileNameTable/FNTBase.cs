using NDSParse.Data;
using NDSParse.Objects.Exports;

namespace NDSParse.Objects.Files.FileNameTable;

public class FNTBase : NDSExport
{
    public Dictionary<ushort, string> FilesById = new();
    public ushort FirstID;
    
    protected const int RootID = 0xF000;

    protected ushort LoadDirectory(BaseReader reader, int folderId = RootID, string folderName = "", string pathAtThisPoint = "")
    {
        pathAtThisPoint = string.IsNullOrEmpty(folderName) ? pathAtThisPoint : pathAtThisPoint + $"{folderName}/";
        reader.Position = (folderId & 0xFF) * 8;
        
        var entryOffset = reader.Read<uint>();
        var fileId = reader.Read<ushort>();
        var parentId = reader.Read<ushort>();

        reader.Position = entryOffset;

        var currentId = fileId;
        while (true)
        {
            var controlByte = reader.ReadByte();
            if (controlByte == 0) break;

            var nameLength = controlByte & 0x7F;
            var name = reader.ReadString(nameLength);
            
            var isFile = (controlByte & 0x80) == 0;
            if (isFile)
            {
                FilesById[currentId] = pathAtThisPoint + name;
                currentId++;
            }
            else
            {
                var subFolderId = reader.Read<ushort>();
                reader.Peek(() => LoadDirectory(reader, subFolderId, name, pathAtThisPoint));
            }
        }

        return fileId;
    }
}