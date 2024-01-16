namespace NDSParse.Conversion.Textures.Pixels.Indexed.Types;

public class A3I5 : PixelTypeBase
{
    public override int BitsPerPixel => 8;

    public override IndexedPixel ProvidePixel(byte data)
    {
        var pixel = new IndexedPixel();
        pixel.Index = (ushort) (data & 0x1F);

        var alpha = (data >> 5) << 3;
        pixel.Alpha = (byte) ((alpha * 4 + alpha / 2) * 8);
        return pixel;
    }
}