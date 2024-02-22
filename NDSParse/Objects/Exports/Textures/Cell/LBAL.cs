using System.Numerics;
using NDSParse.Data;

namespace NDSParse.Objects.Exports.Textures.Cell;

public class LBAL : NDSExport
{
    public List<string> Names = [];
    
    public override string Magic => "LBAL";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        var offsets = new List<uint>();

        var currentOffset = reader.Read<uint>();
        while (currentOffset <= 0xFFFF)
        {
            offsets.Add(currentOffset);
            currentOffset = reader.Read<uint>();
        }

        var labelBeginPosition = reader.Position - sizeof(uint); // skip last lead

        foreach (var offset in offsets)
        {
            reader.Position = labelBeginPosition + offset;
            Names.Add(reader.ReadNullTerminatedString());
        }

    }
}
