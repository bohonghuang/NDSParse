using System.Diagnostics;
using NDSParse.Data;
using NDSParse.Objects;
using NDSParse.Objects.Exports;
using NDSParse.Objects.Files;
using NDSParse.Objects.Files.FileAllocationTable;
using NDSParse.Objects.Files.FileNameTable;
using NDSParse.Objects.Rom;
using Serilog;

namespace NDSParse;

public class NDSProvider
{
    public NDSHeader Header;
    public NDSBanner Banner;
    public Dictionary<string, GameFile> Files = new();

    public bool UnpackNARCs = true;
    
    private BaseReader _reader;
    private FileInfo _file;

    public NDSProvider(FileInfo file)
    {
        _file = file;
    }
    
    public NDSProvider(string path) : this(new FileInfo(path)) { }

    public void Initialize()
    {
        _reader = new BaseReader(File.ReadAllBytes(_file.FullName), _file.Name);
        
        Header = new NDSHeader(_reader);
        Banner = new NDSBanner(Header.BannerData.CreateReader());

        var fileBlocks = new FAT(Header.FATData.CreateReader());
        var fileNames = new FNT(Header.FNTData.CreateReader());
        
        Mount(fileBlocks, fileNames);

        if (UnpackNARCs)
        {
            foreach (var (path, gameFile) in Files.ToArray())
            {
                if (!path.EndsWith(".narc")) continue;
                if (!TryLoadObject<NARC>(path, out var narc)) continue;

                Log.Information("Unpacking NARC: {path}", path);

                var basePath = path.Replace(".narc", string.Empty);
                foreach (var (narcPath, narcGameFile) in narc.Files)
                {
                    // move things in terms of global reader
                    var newPath = basePath + $"/{narcPath}";
                    narcGameFile.Data.Owner = _reader;
                    narcGameFile.Data.Offset += gameFile.Data.Offset;
                    Files[newPath] = narcGameFile;
                }

                Files.Remove(path);
            }
        }
    }
    
    public T LoadObject<T>(string path) where T : NDSObject, new() => LoadObject<T>(Files[path]);

    public T LoadObject<T>(GameFile file) where T : NDSObject, new() => Deserializable.Construct<T>(CreateReader(file));
    
    public bool TryLoadObject<T>(string path, out T data) where T : NDSObject, new() => TryLoadObject(Files[path], out data);
    
    public bool TryLoadObject<T>(GameFile file, out T data) where T : NDSObject, new()
    {
        data = null!;
        try
        {
            data = LoadObject<T>(file);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    public AssetReader CreateReader(GameFile file) => file.CreateReader();
    
    public AssetReader CreateReader(string path) => CreateReader(Files[path]);

    protected void Mount(FAT fat, FNT fnt)
    {
        for (ushort id = 0; id < fat.FileBlocks.Count; id++)
        {
            if (id < fnt.FirstID) continue;

            var fileBlock = fat.FileBlocks[id];
            if (fileBlock.Length <= 0) continue;
            
            var fileName = fnt.FilesById[id];
            if (!fileName.Contains('.'))
            {
                _reader.Position = fileBlock.Offset;
                var extension = _reader.Peek(() => _reader.ReadString(4)).ToLower();
                if (!FileTypeRegistry.Contains(extension)) extension = "bin";

                fileName += $".{extension}";
            }

            var gameFile = new GameFile(fileName, fat.FileBlocks[id]);
            Files[fileName] = gameFile;
        }
    }
}