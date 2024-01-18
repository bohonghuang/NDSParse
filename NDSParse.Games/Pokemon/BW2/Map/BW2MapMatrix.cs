using NDSParse.Data;
using NDSParse.Objects;

namespace NDSParse.Games.Pokemon.BW2.Map;

public class BW2MapMatrix : Deserializable
{
    public ushort Width;
    public ushort Height;

    public List<MapMatrixEntry> MapIndices = [];
    public List<MapMatrixEntry> MapHeaderIndices  = [];
    
    public override void Deserialize(BaseReader reader)
    {
        var hasMapHeaders = reader.Read<int>() == 1;
        Width = reader.Read<ushort>();
        Height = reader.Read<ushort>();

        var matrixCount = Width * Height;

        for (var matrixIndex = 0; matrixIndex < matrixCount; matrixIndex++)
        {
            MapIndices.Add(Construct<MapMatrixEntry>(reader));
        }

        if (hasMapHeaders)
        {
            for (var matrixIndex = 0; matrixIndex < matrixCount; matrixIndex++)
            {
                MapHeaderIndices.Add(Construct<MapMatrixEntry>(reader));
            }
        }
        
    }
}

public class MapMatrixEntry : Deserializable
{
    public bool IsValid => Value != 0xFFFFFFFF;
    public uint Value;
    public override void Deserialize(BaseReader reader)
    {
        Value = reader.Read<uint>();
    }
}