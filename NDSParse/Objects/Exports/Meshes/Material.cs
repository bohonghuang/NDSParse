using NDSParse.Data;

namespace NDSParse.Objects.Exports.Meshes;

public class MDL0Material : NamedDeserializable
{
    public string TextureName;
    public string PaletteName;

    public ushort Tag;
    public ushort Length;
    public int DiffuseAmbient;
    public int SpecularEmissive;
    public uint PolyAttr;
    public uint PolyAttrMask;
    public uint TextureImageParam;
    public uint TextureImageParamMask;
    public ushort TexturePaletteBase;
    public MaterialFlag Flag;
    public ushort Width;
    public ushort Height;
    public float MagWidth;
    public float MagHeight; 
    public uint ScaleU;
    public uint ScaleV;
    public ushort RotSin;
    public ushort RotCos;
    public uint TransU;
    public uint TransV;
    public uint[] EffectMatrix = [];
    
    public override void Deserialize(BaseReader reader)
    {
        Tag = reader.Read<ushort>();
        Length = reader.Read<ushort>();
        DiffuseAmbient = reader.Read<int>();
        SpecularEmissive = reader.Read<int>();
        PolyAttr = reader.Read<uint>();
        PolyAttrMask = reader.Read<uint>();
        TextureImageParam = reader.Read<uint>();
        TextureImageParam = reader.Read<uint>();
        TexturePaletteBase = reader.Read<ushort>();
        Flag = (MaterialFlag) (~reader.Read<ushort>() & 0x3FFF) ^ MaterialFlag.EFFECT_MATRIX;
        Width = reader.Read<ushort>();
        Height = reader.Read<ushort>();
        MagWidth = reader.ReadIntAsFloat();
        MagHeight = reader.ReadIntAsFloat();
        
        if (Flag.HasFlag(MaterialFlag.TEX_MATRIX_SCALEONE))
        {
            ScaleU = reader.Read<uint>();
            ScaleV = reader.Read<uint>();
        }
        if (Flag.HasFlag(MaterialFlag.TEX_MATRIX_ROTZERO))
        {
            RotSin = reader.Read<ushort>();
            RotCos = reader.Read<ushort>();
        }
        if (Flag.HasFlag(MaterialFlag.TEX_MATRIX_TRANSZERO))
        {
            TransU = reader.Read<uint>();
            TransV = reader.Read<uint>();
        }
        
        if (Flag.HasFlag(MaterialFlag.EFFECT_MATRIX))
        {
            EffectMatrix = new uint[0x10];
            for (var i = 0; i < 0x10; i++)
            {
                EffectMatrix[i] = reader.Read<uint>();
            }
        }
    }
}

[Flags]
public enum MaterialFlag : ushort
{
    TEX_MATRIX_USE = 1,
    TEX_MATRIX_SCALEONE = 2,
    TEX_MATRIX_ROTZERO = 4,
    TEX_MATRIX_TRANSZERO = 8,
                
    ORIGWH_SAME = 16, // 0x0010
    WIREFRAME = 32, // 0x0020
    DIFFUSE = 64, // 0x0040
    AMBIENT = 128, // 0x0080
                
    VTXCOLOR = 256, // 0x0100
    SPECULAR = 512, // 0x0200
    EMISSION = 1024, // 0x0400
    SHININESS = 2048, // 0x0800
                
    TEXPLTTBASE = 4096, // 0x1000
    EFFECT_MATRIX = 8192 // 0x2000 
}

public class MaterialMapping : Deserializable
{
    public ushort Offset;
    public byte NumMaterials;
    public byte Bound;

    public override void Deserialize(BaseReader reader)
    {
        Offset = reader.Read<ushort>();
        NumMaterials = reader.ReadByte();
        Bound = reader.ReadByte();
    }
}
