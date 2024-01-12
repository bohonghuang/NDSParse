using NDSParse.Data;
using NDSParse.Objects.Files.FileAllocationTable;

namespace NDSParse.Objects.Exports.Sounds.SoundData;

public class FILE : NDSExport
{
    public uint Count;
    public override string Magic => "FILE";
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        Count = reader.Read<uint>();
    }
}