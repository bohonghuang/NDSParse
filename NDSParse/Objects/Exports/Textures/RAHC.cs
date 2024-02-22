using NDSParse.Conversion.Textures;
using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Colored.Types;
using NDSParse.Conversion.Textures.Pixels.Indexed.Types;
using NDSParse.Data;

namespace NDSParse.Objects.Exports.Textures;

public class RAHC : NDSExport
{
    public IndexedImage Texture;
    
    public ushort NumYTiles;
    public ushort NumXTiles;
    public TextureFormat Format;
    
    public override string Magic => "RAHC";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        NumYTiles = reader.Read<ushort>();
        NumXTiles = reader.Read<ushort>();
        if (NumXTiles == 0xFFFF) NumXTiles = 1;
        if (NumYTiles == 0xFFFF) NumYTiles = 1;
        
        Format = (TextureFormat) reader.Read<uint>();

        reader.Position += 4;
        
        var isTiled = reader.Read<uint>() == 0;
        var pixelDataSize = reader.Read<uint>();
                
        reader.Position += sizeof(uint); // unknown
        
        var width = NumXTiles * 8;
        var height = NumYTiles * 8;

        if (NumXTiles == 0xFFFF || NumYTiles == 0xFFFF)
        {
            var pixelCount = pixelDataSize * 8 / Format.BitsPerPixel();
            var pixelCountSqrt = MathF.Sqrt(pixelCount);
            if (pixelCountSqrt % 1 == 0) // perfect square
            {
                width = (int) pixelCountSqrt;
                height = (int) pixelCountSqrt;
            }
            else
            {
                width = 256;
                height = (int) (pixelCount / 256);
            }
        }
        
        var pixels = Format switch
        {
            TextureFormat.Color4 => reader.ReadPixels<Indexed2BPP>(width, height),
            TextureFormat.Color16 => reader.ReadPixels<Indexed4BPP>(width, height),
            TextureFormat.Color256 => reader.ReadPixels<Indexed8BPP>(width, height),
            TextureFormat.A3I5 => reader.ReadPixels<A3I5>(width, height),
            TextureFormat.A5I3 => reader.ReadPixels<A5I3>(width, height),
            TextureFormat.A1BGR5 => reader.ReadPixels<A1BGR555>(width, height)
        };

        if (isTiled)
        {
            PixelSwizzler.UnSwizzle(ref pixels, width);
        }

        if (Format.IsIndexed())
        {
            Texture = new IndexedImage(pixels, new ImageMetaData(width, height), "Texture");
        }
        else
        {
            throw new NotSupportedException();
            //Texture = new ColoredImage(pixels, new ImageMetaData(width, height), "Texture");
        }
        
    }
}