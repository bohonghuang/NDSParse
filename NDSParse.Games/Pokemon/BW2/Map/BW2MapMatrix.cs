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
        for (uint matrixIndex = 0; matrixIndex < matrixCount; matrixIndex++)
        {
            var entry = Construct<MapMatrixEntry>(reader);
            entry.Y = matrixIndex / Width;
            entry.X = matrixIndex - entry.Y * Width;
            entry.Index = matrixIndex;
            MapIndices.Add(entry);
        }

        if (hasMapHeaders)
        {
            for (uint matrixIndex = 0; matrixIndex < matrixCount; matrixIndex++)
            {
                var entry = Construct<MapMatrixEntry>(reader);
                entry.Y = matrixIndex / Width;
                entry.X = matrixIndex - entry.Y * Width;
                entry.Index = matrixIndex;
                MapHeaderIndices.Add(entry);
            }
        }
        
    }
}

public class MapMatrixEntry : Deserializable
{
    public bool IsValid => Value != 0xFFFFFFFF;
    public uint Value;
    public uint X;
    public uint Y;
    public uint Index;
    public override void Deserialize(BaseReader reader)
    {
        Value = reader.Read<uint>();
    }
}