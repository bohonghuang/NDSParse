using NDSParse.Data;

namespace NDSParse.Objects.Exports.Sounds.SoundData;

public class INFO : SoundRecord<SoundInfoTypeBase>
{
    public override string Magic => "INFO";

    public override SoundInfoTypeBase RecordHandler(BaseReader reader, SoundFileType type) => type switch
    {
        SoundFileType.Stream => Construct<STRMInfo>(reader),
        _ => new SoundInfoTypeBase()
    };
}
