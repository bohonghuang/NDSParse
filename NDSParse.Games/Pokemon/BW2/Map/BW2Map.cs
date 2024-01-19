using System.Numerics;
using NDSParse.Data;
using NDSParse.Objects;
using NDSParse.Objects.Exports.Meshes;

namespace NDSParse.Games.Pokemon.BW2.Map;

public class BW2Map : Deserializable
{
    public BMD0 Model;
    public List<BW2Building> Buildings = [];

    private uint ModelOffset;
    private uint BuildingOffset;
    private uint PermissionsOffset;
    
    public override void Deserialize(BaseReader reader)
    {
        var identifier = reader.ReadString(2);
        reader.Position += 2; // rest of identifier? idc 
        
        ModelOffset = reader.Read<uint>();

        switch (identifier)
        {
            case "GN": // does not contain move permissions
            {
                BuildingOffset = reader.Read<uint>();
                reader.Position += sizeof(uint); // end offset?
                break;
            }
            case "WB":
            case "DR":
            {
                PermissionsOffset = reader.Read<uint>();
                BuildingOffset = reader.Read<uint>();
                reader.Position += sizeof(uint); // end offset?
                break;
            }
            default:
            {
                PermissionsOffset = reader.Read<uint>();
                reader.Position += sizeof(uint); // unknown
                BuildingOffset = reader.Read<uint>();
                reader.Position += sizeof(uint); // end offset?
                break;
            }
        }

        reader.Position = ModelOffset;
        Model = Construct<BMD0>(reader.Spliced());

        reader.Position = BuildingOffset;
        var numBuildings = reader.Read<uint>();
        for (var buildingIndex = 0; buildingIndex < numBuildings; buildingIndex++)
        {
            Buildings.Add(Construct<BW2Building>(reader));
        }
    }
}

public class BW2Building : Deserializable
{
    public Vector3 Location = Vector3.Zero;
    public Vector3 LocationFlags = Vector3.Zero;
    public byte Rotation;
    public int ModelID;
    
    public override void Deserialize(BaseReader reader)
    {
        LocationFlags.X = reader.Read<ushort>();
        Location.X = reader.Read<short>();
        LocationFlags.Y = reader.Read<ushort>();
        Location.Y = reader.Read<short>();
        LocationFlags.Z = reader.Read<ushort>();
        Location.Z = reader.Read<short>();
        reader.Position += 1; // unknown
        Rotation = reader.ReadByte();
        ModelID = (reader.ReadByte() << 8) + reader.ReadByte();
    }
}