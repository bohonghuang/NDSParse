using NDSParse.Conversion.Textures.Images;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Data;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = NDSParse.Conversion.Textures.Colors.Color;

namespace NDSParse.Objects.Exports.Fonts;

public class RTFN : NDSObject
{
    public List<Character> Characters = [];
    
    public FNIF FontInfo;
    public PLGC Bitmaps;
    public HDWC Widths;
    public PAMC[] Maps;
    
    public override string Magic => "RTFN";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        FontInfo = GetBlock<FNIF>();
        Bitmaps = GetBlock<PLGC>();
        Widths = GetBlock<HDWC>();
        Maps = GetBlocks<PAMC>();
        

        foreach (var characterMap in Maps)
        {
            foreach (var (index, character) in characterMap.Map)
            {
                Characters.Add(new Character
                {
                    Index = index,
                    CharCode = character,
                    Image = GetChar(Bitmaps.Tiles[index], Bitmaps.Depth, Bitmaps.BoxWidth, Bitmaps.BoxHeight, Bitmaps.Palette),
                    WidthInfo = Widths.IsValid ? Widths.Infos[index] : Bitmaps.EmbeddedWidthInfos[index]
                });
            } 
        }
    }
    
    private Image<Rgba32> GetChar(byte[] tiles, int depth, int width, int height, Palette palette)
    {
        var image = new Image<Rgba32>(width, height);
        var tileData = new List<byte>();

        for (var i = 0; i <= tiles.Length - depth; i += depth)
        {
            byte byteFromBits = 0x00;
            for (int b = depth - 1, j = 0; b >= 0; b--, j++)
            {
                byteFromBits += (byte)(tiles[i + j] << b);
            }
            tileData.Add(byteFromBits);
        }
        
        image.IteratePixels((ref Rgba32 pixel, int index) =>
        {
            pixel = palette.Colors[tileData[index]].ToPixel<Rgba32>();
        });

        return image;
    }
}

public class Character
{
    public int Index;
    public ushort CharCode;
    public Image<Rgba32> Image;
    public WidthInfo WidthInfo;
}