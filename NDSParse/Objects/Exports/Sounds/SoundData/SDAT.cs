using NDSParse.Data;

namespace NDSParse.Objects.Exports.Sounds.SoundData;

public class SDAT : NDSObject
{
    public SYMB Symbols;
    public INFO Info;
    public FAT FileBlocks;
    public FILE FileInfo;
    
    public Dictionary<SoundFileType, NDSObject> Files = new();
    
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

        Symbols = Construct<SYMB>(reader.Spliced(SymbOffset, SymbSize));
        Info = Construct<INFO>(reader.Spliced(InfoOffset, InfoSize));
        FileBlocks = Construct<FAT>(reader.Spliced(FATOffset, FATSize));
        FileInfo = Construct<FILE>(reader.Spliced(FilesOffset, FilesSize));
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