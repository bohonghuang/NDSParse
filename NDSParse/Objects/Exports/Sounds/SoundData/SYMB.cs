using NDSParse.Data;

namespace NDSParse.Objects.Exports.Sounds.SoundData;

public class SYMB : SoundRecord<string>
{
    public override string Magic => "SYMB";

    public override string RecordHandler(BaseReader reader, SoundFileType type) => reader.ReadNullTerminatedString();
}