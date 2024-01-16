namespace NDSParse.Conversion.Textures.Pixels.Indexed.Types;

public class A5I3 : PixelTypeBase
{
    public override int BitsPerPixel => 8;

    public override IndexedPixel ProvidePixel(byte data)
    {
        var pixel = new IndexedPixel();
        pixel.Index = (ushort) (data & 0x7);
        pixel.Alpha = (byte) (((data >> 3) << 5) * 8);
        return pixel;
    }
}