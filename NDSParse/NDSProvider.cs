using System.Diagnostics;
using NDSParse.Data;
using NDSParse.Objects;
using NDSParse.Objects.Exports;
using NDSParse.Objects.Exports.Archive;
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
            foreach (var narc in LoadAllFilesOfType<NARC>())
            {
                narc.MountToProvider(this);
                Files.Remove(narc.Path);
            }
        }
    }
    
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

    public IEnumerable<GameFile> GetAllFilesOfType<T>() where T : NDSObject, new()
    {
        var objectRef = new T();
        return Files.Values.Where(file => file.Type.Equals(objectRef.Magic, StringComparison.OrdinalIgnoreCase));
    }
    
    public IEnumerable<T> LoadAllFilesOfType<T>() where T : NDSObject, new()
    {
        var loaded = new List<T>();
        foreach (var file in GetAllFilesOfType<T>())
        {
            if (TryLoadObject(file, out T data))
            {
                loaded.Add(data);
            }
        }

        return loaded;
    }
    
    public T LoadObject<T>(string path) where T : Deserializable, new() => LoadObject<T>(Files[path]);

    public T LoadObject<T>(GameFile file) where T : Deserializable, new() => Deserializable.Construct<T>(CreateReader(file));
    
    public bool TryLoadObject<T>(string path, out T data) where T : Deserializable, new() => TryLoadObject(Files[path], out data);
    
    public bool TryLoadObject<T>(GameFile file, out T data) where T : Deserializable, new()
    {
        data = null!;
        data = LoadObject<T>(file);
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
}