namespace NDSParse.Conversion.Textures.Pixels.Types;

public class Alpha3Indexed5 : PixelTypeBase
{
    public override int BitsPerPixel => 8;

    public override IndexedPixel ProvidePixel(byte data)
    {
        var pixel = new IndexedPixel();
        pixel.Index = (ushort) (data & 0x1F);

        var alpha = data << 5;
        pixel.Alpha = (byte) ((alpha * 4 + alpha / 2) * 8);
        return pixel;
    }
}