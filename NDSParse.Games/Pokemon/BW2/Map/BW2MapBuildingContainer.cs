using System.Numerics;
using NDSParse.Data;
using NDSParse.Objects;
using NDSParse.Objects.Exports;
using NDSParse.Objects.Exports.Meshes;

namespace NDSParse.Games.Pokemon.BW2.Map;

public class BW2MapBuildingContainer : Deserializable
{
    public ushort FileCount;
    public List<BW2MapBuildingHeader> Headers = [];
    public List<BMD0> Models = [];
    
    private List<uint> FileOffsets = [];
    
    private const string Magic = "AB";
    
    public override void Deserialize(BaseReader reader)
    {
        var magic = reader.ReadString(2);
        if (magic != Magic)
        {
            throw new ParserException($"{Magic} has invalid magic {magic}!");
        }

        FileCount = reader.Read<ushort>();
        for (var fileIndex = 0; fileIndex < FileCount; fileIndex++)
        {
            FileOffsets.Add(reader.Read<uint>());
        }

        // i dont understand the offsets but it works, dont @ me
        for (var offsetIndex = 0; offsetIndex < FileCount / 2; offsetIndex++)
        {
            reader.Position = FileOffsets[offsetIndex];
            Headers.Add(Construct<BW2MapBuildingHeader>(reader.Spliced()));

            /*var headerType = reader.ReadByte();
            if (headerType == 0)
            {
                reader.Position = FileOffsets[offsetIndex];
                Headers.Add(new DataBlock(reader, (int) reader.Position, 36));
            }
            else
            {
                reader.Position += headerType switch
                {
                    1 => 0,
                    2 => 4,
                    3 => 8,
                    4 => 12
                };

                var offset = reader.Read<uint>();
                reader.Position = FileOffsets[offsetIndex] + 24 + offset;
                var size = reader.Read<uint>();
                reader.Position = FileOffsets[offsetIndex];
                
                Headers.Add(new DataBlock(reader, (int) reader.Position, (int) (offset + size + 16)));
            }*/
        }
        
        for (var offsetIndex = FileCount / 2; offsetIndex < FileCount; offsetIndex++)
        {
            reader.Position = FileOffsets[offsetIndex];

            Models.Add(Construct<BMD0>(reader.Spliced()));
            
        }
    }
}

public class BW2MapBuildingHeader : Deserializable
{
    public ushort ID;
    public short DoorID;
    public Vector3 DoorLocation = Vector3.Zero;

    public List<NDSObject> GenericHeaders = []; // todo actually serialize, just placeholder
    
    private const int MaxHeaderSubfiles = 4;
    private const int OffsetZeroPoint = 16;
    
    public override void Deserialize(BaseReader reader)
    {
        ID = reader.Read<ushort>();
        
        reader.Position += 2;
        
        DoorID = reader.Read<short>();
        DoorLocation.X = reader.Read<short>();
        DoorLocation.Y = reader.Read<short>();
        DoorLocation.Z = reader.Read<short>();
        
        reader.Position += 7; // unknown rn

        var subFileOffsets = new List<uint>();
        var filesInHeader = reader.ReadByte();
        for (var subFileOffsetIndex = 0; subFileOffsetIndex < MaxHeaderSubfiles; subFileOffsetIndex++)
        {
            var offset = reader.Read<uint>();
            if (subFileOffsetIndex < filesInHeader)
            {
                subFileOffsets.Add(offset + OffsetZeroPoint);
            }
        }

        foreach (var subFileOffset in subFileOffsets)
        {
            reader.Position = subFileOffset;
            GenericHeaders.Add(NDSObject.DeserializeGenericHeader(reader.Spliced())); // for reading purposes
        }
    }
}