using NDSParse.Conversion.Textures.Colors.Types;

namespace NDSParse.Conversion.Textures.Pixels.Colored.Types;

public class A1BGR555 : PixelTypeBase
{
    public override int BitsPerPixel => 16;

    public override IPixel[] Decode(byte[] data)
    {
        var pixelCount = data.Length / 2;
        
        var pixels = new IPixel[pixelCount];
        for (int pixelIndex = 0, byteIndex = 0; pixelIndex < pixelCount; pixelIndex++, byteIndex += 2)
        {
            var shortValue = BitConverter.ToUInt16(data, byteIndex);
            var color = BGR555.Read(shortValue);
            color.A = (byte) (shortValue >> 15 == 1 ? 255 : 0);
            pixels[pixelIndex] = new ColoredPixel(color);
        }
        
        return pixels;
    }
    
}