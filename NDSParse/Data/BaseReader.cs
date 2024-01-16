using System.Text;
using GenericReader;
using NDSParse.Extensions;

namespace NDSParse.Data;

public class BaseReader : GenericBufferReader
{
    public string Name;
    public BaseReader Owner;
    public virtual BaseReader AbsoluteOwner
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

    public BaseReader(byte[] buffer, string name = "", BaseReader? owner = null) : base(buffer)
    {
        Name = name;
        Owner = owner ?? this;
    }

    public BaseReader Spliced(uint? position = null, uint? length = null)
    {
        Position = position ?? Position;
        length ??= (uint) (Size - Position);
        return new BaseReader(ReadBytes((int) length));
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
    
    public T ReadEnum<T, K>() where T : Enum where K : unmanaged
    {
        return (T) (object) Read<K>();
    }

    public byte[] ReadAllBytes()
    {
        Position = 0;
        return ReadBytes(Size);
    }
    
    public T Peek<T>(Func<T> func)
    {
        var originalPos = Position;
        var ret = func();
        Position = originalPos;
        return ret;
    }
    
    public void Peek(Action func)
    {
        var originalPos = Position;
        func();
        Position = originalPos;
    }

    public float ReadIntAsFloat() => FloatExtensions.ToFloat(Read<int>(), 1, 19, 12);
    public float ReadShortAsFloat() => FloatExtensions.ToFloat(Read<ushort>(), 1, 3, 12);
}