using System.Numerics;
using NDSParse.Data;

namespace NDSParse.Objects.Exports.Textures.Cell;

public class KBEC : NDSExport
{
    public List<CellBank> Banks = [];
    
    public ushort BankCount;
    public BankType BankType;
    public byte BlockSize;

    private uint DataOffset;
    private uint PartitionDataOffset;
    
    public override string Magic => "KBEC";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        BankCount = reader.Read<ushort>();
        BankType = reader.ReadEnum<BankType, ushort>();

        DataOffset = reader.Read<uint>();
        BlockSize = reader.ReadByte();

        reader.Position += 3;

        var partitionDataOffset = reader.Read<uint>();
        if (partitionDataOffset != 0)
        {
            throw new NotImplementedException("RECN Partition Data is not Supported");
        }
        
        reader.Position = DataOffset + HEADER_SIZE;

        var bankInfos = new List<BankInfo>();
        for (var bankIndex = 0; bankIndex < BankCount; bankIndex++)
        {
            bankInfos.Add(Construct<BankInfo>(reader, info => info.BankType = BankType));
        }
        
        for (var bankIndex = 0; bankIndex < BankCount; bankIndex++)
        {
            Banks.Add(Construct<CellBank>(reader, info => info.Info = bankInfos[bankIndex]));
        }
        
        
    }
}

public enum BankType : ushort
{
    Default = 0,
    Bounds = 1
}

public class BankInfo : Deserializable
{
    public ushort CellCount;
    public ushort ReadOnlyCellInfo;
    public uint CellOffset;

    public Vector2 MinBounds = Vector2.Zero;
    public Vector2 MaxBounds = Vector2.Zero;
    
    public BankType BankType;
    
    public override void Deserialize(BaseReader reader)
    {
        CellCount = reader.Read<ushort>();
        ReadOnlyCellInfo = reader.Read<ushort>();
        CellOffset = reader.Read<uint>();

        if (BankType == BankType.Bounds)
        {
            MaxBounds = new Vector2(reader.Read<short>(), reader.Read<short>());
            MinBounds = new Vector2(reader.Read<short>(), reader.Read<short>());
        }
        
    }
}

public class CellBank : Deserializable
{
    public List<Cell> Cells = [];
    public BankInfo Info;
    
    public override void Deserialize(BaseReader reader)
    {
        for (var cellIndex = 0; cellIndex < Info.CellCount; cellIndex++)
        {
            Cells.Add(Construct<Cell>(reader));
        }
    }
}

public class Cell : Deserializable
{
    public int Width;
    public int Height;
    
    public int XOffset;
    public int YOffset;

    public int TileOffset;
    public byte PaletteIndex;

    public CellBitDepth BitDepth;
    public CellShape Shape;
    public CellScale Scale;
    
    
    public override void Deserialize(BaseReader reader)
    {
        var firstFlag = reader.Read<ushort>();

        YOffset = (sbyte) (firstFlag & 0xFF);
        BitDepth = (CellBitDepth) ((firstFlag >> 13) & 1);
        Shape = (CellShape) (firstFlag >> 14);

        var secondFlag = reader.Read<ushort>();
        
        XOffset = (sbyte) (secondFlag & 0x1FF);
        Scale = (CellScale) (secondFlag >> 14);

        var thirdFlag = reader.Read<ushort>();

        TileOffset = thirdFlag & 0x3FF;
        PaletteIndex = (byte) (thirdFlag >> 12);

        var dimensions = GetDimensions(Shape, Scale);
        Width = (int) dimensions.X;
        Height = (int) dimensions.Y;
    }

    public static Vector2 GetDimensions(CellShape shape, CellScale scale)
    {
        return shape switch
        {
            CellShape.Square => scale switch
            {
                CellScale.Small => new Vector2(8, 8),
                CellScale.Medium => new Vector2(16, 16),
                CellScale.Large => new Vector2(32, 32),
                CellScale.XLarge => new Vector2(64, 64)
            },
            
            CellShape.HorizontalRectangle => scale switch
            {
                CellScale.Small => new Vector2(16, 8),
                CellScale.Medium => new Vector2(32, 8),
                CellScale.Large => new Vector2(32, 16),
                CellScale.XLarge => new Vector2(64, 32)
            },
            
            CellShape.VerticalRectangle => scale switch
            {
                CellScale.Small => new Vector2(8, 16),
                CellScale.Medium => new Vector2(16, 32),
                CellScale.Large => new Vector2(16, 32),
                CellScale.XLarge => new Vector2(32, 64)
            }
        };
    }
}

public enum CellBitDepth
{
    Bit4,
    Bit8
}

public enum CellShape
{
    Square,
    HorizontalRectangle,
    VerticalRectangle
}

public enum CellScale
{
    Small,
    Medium,
    Large,
    XLarge,
}
