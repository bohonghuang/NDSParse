using NDSParse.Data;
using NDSParse.Objects.Files.FileAllocationTable;

namespace NDSParse.Objects.Exports.Sounds.SoundData;

public class FAT : FATBase
{
    public override string Magic => "FAT";
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        var count = reader.Read<uint>();
        for (var i = 0; i < count; i++)
        {
            FileBlocks.Add(new DataBlock(reader));
            reader.Position += sizeof(uint) * 2; // reserved
        }
    }
}