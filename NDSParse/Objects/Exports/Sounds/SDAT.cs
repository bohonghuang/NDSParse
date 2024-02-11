using NDSParse.Data;
using NDSParse.Objects.Exports.Sounds.SoundData;
using NDSParse.Objects.Files;

namespace NDSParse.Objects.Exports.Sounds;

public class SDAT : NDSObject
{
    public List<STRM> Streams = [];
    
    public SYMB Symbols;
    public INFO Info;
    public FAT FAT;
    public FILE FileInfo;
    
    public override string Magic => "SDAT";

    private uint SymbOffset;
    private uint SymbSize;
    private uint InfoOffset;
    private uint InfoSize;
    private uint FATOffset;
    private uint FATSize;
    private uint FilesOffset;
    private uint FilesSize;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        SymbOffset = reader.Read<uint>();
        SymbSize = reader.Read<uint>();
        InfoOffset = reader.Read<uint>();
        InfoSize = reader.Read<uint>();
        FATOffset= reader.Read<uint>();
        FATSize = reader.Read<uint>();
        FilesOffset = reader.Read<uint>();
        FilesSize = reader.Read<uint>();

        Symbols = ConstructExport<SYMB>(reader.Spliced(SymbOffset, SymbSize));
        Info = ConstructExport<INFO>(reader.Spliced(InfoOffset, InfoSize));
        FAT = ConstructExport<FAT>(reader.Spliced(FATOffset, FATSize));
        FileInfo = ConstructExport<FILE>(reader.Spliced(FilesOffset, FilesSize));

        Streams = LoadFiles<STRM, STRMInfo>(SoundFileType.Stream);
    }

    public List<T> LoadFiles<T, K>(SoundFileType type) where T : SoundTypeBase<K>, new() where K : SoundInfoTypeBase
    {
        var symbols = Symbols.Records[type];
        var infos = Info.Records[type];

        var files = new List<T>();
        for (ushort index = 0; index < symbols.Count; index++)
        {
            var info = infos[index];
            var data = FAT.FileBlocks[info.FileID];
            
            var file = Construct<T>(data.CreateAssetReader());
            file.File = new FileBase(symbols[index]);
            file.Info = (K) info;
            files.Add(file);
        }

        return files;
    }
}

public enum SoundFileType
{
    Sequence,
    SequenceArchive,
    Bank,
    WaveArchive,
    Player,
    Group,
    SoundPlayer,
    Stream
}