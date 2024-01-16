using System.Numerics;
using NDSParse.Data;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Textures.Colors.Types;

public class BGR555 : ColorTypeBase
{
    public override Color Read(BaseReader reader)
    {
        return Read(reader.Read<ushort>());
    }

    public static Color Read(ushort value)
    {
        var r = (byte) (((value >> 0) & 0x1F) << 3);
        var g = (byte) (((value >> 5) & 0x1F) << 3);
        var b = (byte) (((value >> 10) & 0x1F) << 3);
        return new Color(r, g, b);
    }
}