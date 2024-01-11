namespace NDSParse.Conversion.Textures.Pixels.Types;

public abstract class PixelTypeBase
{
    public abstract int BitsPerPixel { get; }

    public IndexedPixel[] Decode(byte[] data)
    {
        var pixelCount = data.Length * (8 / BitsPerPixel);
        var pixels = new IndexedPixel[pixelCount];
        
        var bitMask = (1 << BitsPerPixel) - 1;
        for (int pixelIndex = 0, bitIndex = 0; pixelIndex < pixelCount; pixelIndex++, bitIndex += BitsPerPixel) 
        {
            var value = (data[bitIndex / 8] >> bitIndex % 8) & bitMask;
            pixels[pixelIndex] = ProvidePixel((byte) value);
        }

        return pixels;
    }

    public virtual IndexedPixel ProvidePixel(byte data)
    {
        return new IndexedPixel(data);
    }
}