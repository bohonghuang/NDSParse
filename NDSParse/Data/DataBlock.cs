using NDSParse.Objects.Files;

namespace NDSParse.Data;

public class DataBlock
{
    public int Offset;
    public int Length;
    public BaseReader Owner;

    public DataBlock(BaseReader reader, int offset, int length)
    {
        Owner = reader.AbsoluteOwner;
        Offset = offset;
        Length = length;
    }
    
    public DataBlock(BaseReader reader, DataBlockReadType chunkStyle = DataBlockReadType.OffsetLength)
    {
        Owner = reader.AbsoluteOwner;
        switch (chunkStyle)
        {
            case DataBlockReadType.OffsetLength:
                Offset = reader.Read<int>();
                Length = reader.Read<int>();
                break;
            case DataBlockReadType.StartEnd:
                Offset = reader.Read<int>();
                var endOffset = reader.Read<int>();
                Length = endOffset - Offset;
                break;
            case DataBlockReadType.Length:
                Offset = (int) reader.Position;
                Length = reader.Read<int>();
                break;
        }
    }
    
    public BaseReader CreateReader(string name = "")
    {
        return new BaseReader(GetBytes(), name) { Owner = Owner };
    }
    
    public AssetReader CreateAssetReader(GameFile file)
    {
        return new AssetReader(GetBytes(), file);
    }
    
    private byte[] GetBytes()
    {
        var previousPosition = (int) Owner.Position; 
        Owner.Seek(Offset, SeekOrigin.Begin);

        var data = Owner.ReadBytes(Length);
        Owner.Seek(previousPosition, SeekOrigin.Begin);
        return data;
    }
}

public enum DataBlockReadType
{
    OffsetLength,
    StartEnd,
    Length
}