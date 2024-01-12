using NDSParse.Data;

namespace NDSParse.Objects.Exports.Sounds.SoundData;

public class INFO : SoundRecord<SoundInfo>
{
    public override string Magic => "INFO";

    public override SoundInfo RecordHandler(BaseReader reader, SoundFileType type) => type switch
    {
        SoundFileType.Stream => Construct<STRMInfo>(reader),
        _ => new SoundInfo()
    };
}

public class SoundInfo : Deserializable
{
    public override void Deserialize(BaseReader reader) { }
}

public class FileSoundInfo : SoundInfo
{
    public ushort FileID;
    public override void Deserialize(BaseReader reader)
    {
        FileID = reader.Read<ushort>();
    }
}