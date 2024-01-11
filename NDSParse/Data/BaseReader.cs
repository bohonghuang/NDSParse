using System.Text;
using GenericReader;

namespace NDSParse.Data;

public class BaseReader : GenericBufferReader
{
    public string Name;
    public BaseReader Owner;
    public BaseReader AbsoluteOwner
    {
        get
        {
            var nextOwner = this;
            while (nextOwner.Owner != nextOwner)
            {
                nextOwner = nextOwner.Owner;
            }

            return nextOwner;
        }
    }

    public BaseReader(byte[] buffer, string name = "") : base(buffer)
    {
        Name = name;
        Owner = this;
    }
    
    public string ReadString(int length, bool unicode = false)
    {
        return ReadString(length, unicode ? Encoding.Unicode : Encoding.UTF8).Replace("\0", string.Empty);
    }
    
    public string ReadNullTerminatedString(bool unicode = false)
    {
        var originalPos = Position;
        var length = 0;
        byte currentByte;
        do
        {
            currentByte = Read<byte>();
            length++;
        } while (currentByte != 0x00);

        Position = originalPos;
        return ReadString(length, unicode);
    }
    
    public T ReadEnum<T>() where T : Enum
    {
        return (T) (object) ReadByte();
    }

    public byte[] ReadAllBytes()
    {
        Position = 0;
        return ReadBytes(Size);
    }
}