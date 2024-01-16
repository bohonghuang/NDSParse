using NDSParse.Conversion.Textures.Pixels.Indexed;

namespace NDSParse.Conversion.Textures.Pixels;

public abstract class PixelTypeBase
{
    public virtual int BitsPerPixel => 8;

    public virtual IPixel[] Decode(byte[] data)
    {
        var pixelCount = data.Length * (8 / BitsPerPixel);
        var pixels = new IPixel[pixelCount];
        
        var bitMask = (1 << BitsPerPixel) - 1;
        for (int pixelIndex = 0, bitIndex = 0; pixelIndex < pixelCount; pixelIndex++, bitIndex += BitsPerPixel) 
        {
            var value = (data[bitIndex / 8] >> bitIndex % 8) & bitMask;
            pixels[pixelIndex] = ProvidePixel((byte) value);
        }

        return pixels;
    }

    public virtual IPixel ProvidePixel(byte data)
    {
        return new IndexedPixel(data);
    }
}