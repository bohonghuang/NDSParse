using NDSParse.Data;

namespace NDSParse.Objects.Exports.Meshes;

public class MDL0RenderCommand : Deserializable
{
    public RenderCommandOpCode OpCode;
    public int Flags;
    public byte[] Parameters = [];
    
    public override void Deserialize(BaseReader reader)
    {
        var data = reader.ReadByte();
        OpCode = (RenderCommandOpCode) (data & 0b11111);
        Flags = data >> 5;

        var parameterCount = GetParameterCount(OpCode, Flags, reader);
        if (parameterCount > 0)
        {
            Parameters = reader.ReadBytes(parameterCount);
        }

    }

    public static int GetParameterCount(RenderCommandOpCode command, int flag, BaseReader reader)
    {
        if (command == RenderCommandOpCode.SKIN)
        {
            reader.Position += 1;
            return 2 + reader.Peek(reader.ReadByte) * 3;
        }
        
        return command switch
        {
            RenderCommandOpCode.NOP => 0,
            RenderCommandOpCode.END => 0,
            RenderCommandOpCode.VISIBILITY => 2,
            RenderCommandOpCode.MTX_RESTORE => 1,
            RenderCommandOpCode.BIND_MATERIAL => 1,
            RenderCommandOpCode.DRAW_MESH => 1,
            RenderCommandOpCode.MTX_MULT => flag switch
            {
                0 => 3,
                1 => 4,
                2 => 4,
                3 => 5,
                _ => 0
            },
            RenderCommandOpCode.UNKNOWN_7 => 1,
            RenderCommandOpCode.UNKNOWN_8 => 1,
            RenderCommandOpCode.UNKNOWN_10 => 0,
            RenderCommandOpCode.MTX_SCALE => 0,
            RenderCommandOpCode.UNKNOWN_12 => 0,
            RenderCommandOpCode.UNKNOWN_13 => 0
        };
    }
}

public enum RenderCommandOpCode
{
    NOP = 0x0,
    END = 0x1,
    VISIBILITY = 0x2,
    MTX_RESTORE = 0x3,
    BIND_MATERIAL = 0x4,
    DRAW_MESH = 0x5,
    MTX_MULT = 0x6,
    UNKNOWN_7 = 0x7,
    UNKNOWN_8 = 0x8,
    SKIN = 0x9,
    UNKNOWN_10 = 0xA,
    MTX_SCALE = 0xB,
    UNKNOWN_12 = 0xC,
    UNKNOWN_13 = 0xD,
}