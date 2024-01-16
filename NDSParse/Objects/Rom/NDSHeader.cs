using NDSParse.Data;

namespace NDSParse.Objects.Rom;

public class NDSHeader
{
    public string Title;
    public string GameCode;
    public string MakerCode;
    public DeviceCompatibility DeviceCompatibility;
    public byte EncryptionSeed;
    public long CartridgeSize;
    public byte Version;
    public DataBlock FNTData;
    public DataBlock FATData;
    public DataBlock BannerData;
    
    private const int BaseCartridgeSize = 0x1F400;
    
    public NDSHeader(BaseReader reader)
    {
        // 0x00
        Title = reader.ReadString(12);
        GameCode = reader.ReadString(4);
        
        // 0x20
        MakerCode = reader.ReadString(2);
        DeviceCompatibility = reader.ReadEnum<DeviceCompatibility, byte>();
        EncryptionSeed = reader.ReadByte();
        CartridgeSize = BaseCartridgeSize * (1 << reader.ReadByte());
        reader.Position += 9; // reserved + region stuff
        Version = reader.ReadByte();
        reader.Position += sizeof(byte); // auto start flag
        
        // 0x30
        reader.Position += sizeof(int) * 8; // arm9 + arm7 offsets
        
        // 0x40
        FNTData = new DataBlock(reader);
        FATData = new DataBlock(reader);
        
        // 0x50
        reader.Position += sizeof(int) * 4; // overlay9 + overlay7
        
        // 0x60
        reader.Position += sizeof(int) * 2; // flags

        BannerData = new DataBlock(reader, reader.Read<int>(), NDSBanner.Length);
    }
}

public enum DeviceCompatibility : byte
{
    DS = 0,
    DSiEnhanced = 2,
    DSiExclusive = 3
}

[Flags]
public enum Region : uint
{
    None = 0,
    Japan = 1 << 0,
    USA = 1 << 1,
    Europe = 1 << 2,
    Australia = 1 << 3,
    China = 1 << 4,
    Korea = 1 << 5,
    Unlocked = 0xFFFFFFFF
}