using NDSParse.Data;

namespace NDSParse.Objects.Exports.Meshes;

public class MDL0 : NDSExport
{
    public List<MDL0Model> Models = [];
    
    public override string Magic => "MDL0";
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        var modelOffsets = new NameListUnmanaged<uint>(reader);
        foreach (var (name, offset) in modelOffsets)
        {
            reader.Position = offset;
            
            var model = Construct<MDL0Model>(reader.Spliced());
            model.Name = name;
            Models.Add(model);
        }
    }
}

public class MDL0Model : NamedDeserializable
{
    public List<MDLOMeshInfo> MeshInfos = [];
    public List<MDL0RenderCommand> RenderCommands = [];
    public List<MDL0Material> Materials = [];
    public List<MDL0Polygon> Polygons = [];

    public uint Size;
    public uint RenderCommandsOffset;
    public uint MaterialsOffset;
    public uint PolgygonsOffset;
    public uint InvBindsOffset;

    public byte NumObjects;
    public byte NumMaterials;
    public byte NumPolygons;
    public ushort NumVertices;
    public ushort NumFaces;
    public ushort NumTriangles;
    public ushort NumQuads;

    public float UpScale;
    public float DownScale;
    
    public override void Deserialize(BaseReader reader)
    {
        ReadHeader(reader);
        ReadModelInfo(reader);

        ReadMeshInfos(reader.Spliced());
        
        reader.Position = RenderCommandsOffset;
        ReadRenderCommands(reader.Spliced());

        reader.Position = MaterialsOffset;
        ReadMaterials(reader.Spliced());

        reader.Position = PolgygonsOffset;
        ReadPolygons(reader.Spliced());
    }

    private void ReadHeader(BaseReader reader)
    {
        Size = reader.Read<uint>();
        RenderCommandsOffset = reader.Read<uint>();
        MaterialsOffset = reader.Read<uint>();
        PolgygonsOffset = reader.Read<uint>();
        InvBindsOffset = reader.Read<uint>();
    }
    
    private void ReadModelInfo(BaseReader reader)
    {
        reader.Position += 3; // unknown

        NumObjects = reader.ReadByte();
        NumMaterials = reader.ReadByte();
        NumPolygons = reader.ReadByte();
        
        reader.Position += 2; // unknown

        UpScale = reader.ReadIntAsFloat();
        DownScale = reader.ReadIntAsFloat();
        
        NumVertices = reader.Read<ushort>();
        NumFaces = reader.Read<ushort>();
        NumTriangles = reader.Read<ushort>();
        NumQuads = reader.Read<ushort>();

        reader.Position += sizeof(ushort) * 6; // bounding box min/max
        reader.Position += sizeof(uint) * 2; // bounding box up/down scale
    }

    private void ReadMeshInfos(BaseReader reader)
    {
        var meshOffsets = new NameListUnmanaged<uint>(reader);
        foreach (var (name, offset) in meshOffsets)
        {
            reader.Position = offset;

            var mesh = Construct<MDLOMeshInfo>(reader);
            mesh.Name = name;
            MeshInfos.Add(mesh);
        }
    }

    private void ReadRenderCommands(BaseReader reader)
    {
        MDL0RenderCommand command;
        do
        {
            command = Construct<MDL0RenderCommand>(reader);
            RenderCommands.Add(command);
        }
        while (command.OpCode != RenderCommandOpCode.END);
    }
    
    private void ReadMaterials(BaseReader reader)
    {
        var textureOffset = reader.Read<ushort>();
        var paletteOffset = reader.Read<ushort>();
        
        var materialOffsets = new NameListUnmanaged<uint>(reader);

        reader.Position = textureOffset;
        var textureToMaterialIndex = GetMaterialMapping();
        
        reader.Position = paletteOffset;
        var paletteToMaterialIndex = GetMaterialMapping();

        for (byte matIndex = 0; matIndex < materialOffsets.Count; matIndex++)
        {
            var (name, offset) = materialOffsets.Get(matIndex);
            reader.Position = offset;
            
            var material = Construct<MDL0Material>(reader);
            material.Name = name;
            material.TextureName = textureToMaterialIndex.GetKeyFromValue(matIndex);
            material.PaletteName = paletteToMaterialIndex.GetKeyFromValue(matIndex);
            Materials.Add(material);
        }

        return;

        NameListUnmanaged<byte> GetMaterialMapping() =>
            new(reader, () =>
            {
                var offset = reader.Read<ushort>();
                reader.Position += 2; // num mats + bounds
                return reader.Peek(() =>
                {
                    reader.Position = offset;
                    return reader.ReadByte();
                });
            });
    }

    private void ReadPolygons(BaseReader reader)
    {
        var polygonOffsets = new NameListUnmanaged<uint>(reader);
        var polygonInfos = reader.ReadArray(polygonOffsets.Count, () => Construct<MDL0PolygonInfo>(reader));

        for (var polygonIndex = 0; polygonIndex < polygonInfos.Length; polygonIndex++)
        {
            var info = polygonInfos[polygonIndex];
            var (name, _) = polygonOffsets.Get(polygonIndex);
            var polygon = new MDL0Polygon { Name = name, Info = info };

            var commandEnd = reader.Position + info.CommandLength;
            while (reader.Position < commandEnd)
            {
                var commands = reader.ReadArray(4, reader.ReadEnum<PolygonCommandOpCode, byte>);
                foreach (var command in commands)
                {
                    var parameters = reader.ReadArray<int>(MDL0PolygonCommand.GetParameterCount(command));
                    polygon.Commands.Add(new MDL0PolygonCommand(command, parameters));
                }

            }
            reader.Position = commandEnd;
            Polygons.Add(polygon);
        }
    }
}

