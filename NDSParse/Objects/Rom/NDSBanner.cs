using NDSParse.Conversion.Textures.Colors;
using NDSParse.Conversion.Textures.Colors.Types;
using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Types;
using NDSParse.Data;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Objects.Rom;

public class NDSBanner
{
    public Version Version;
    public IndexedPaletteImage Icon;
    public AnimatedBannerIcon AnimatedIcon;
    public LocalizedBannerTitles Titles;
    
    public const int Length = 0x23C0;
    public const int IconWidth = 32;
    public const int IconHeight = 32;

    public NDSBanner(BaseReader reader)
    {
        var majorVersion = reader.Read<byte>();
        var minorVersion = reader.Read<byte>();
        Version = new Version(majorVersion, minorVersion);
        
        reader.Position += 30; // checksums + reserved
        
        Icon = ReadIcon(reader);
        Titles = new LocalizedBannerTitles(reader, Version);

        reader.Position = 0x1240;

        AnimatedIcon = new AnimatedBannerIcon(reader);
    }

    private IndexedPaletteImage ReadIcon(BaseReader reader)
    {
        var pixels = reader.ReadPixels<Indexed4BPP>(IconWidth, IconHeight);
        PixelSwizzler.UnSwizzle(ref pixels, IconWidth);
        var palette = new Palette(reader.ReadColors<BGR555>(16));

        return new IndexedPaletteImage(IconWidth, IconHeight, pixels, [palette]);
    }

}

public class AnimatedBannerIcon
{
    public List<IndexedImage> Images = [];
    public List<Palette> Palettes = [];
    public List<AnimatedBannerKey> Keys = [];
    public readonly int Width = 32;
    public readonly int Height = 32;
    
    private const int AnimatedImageCount = 8;
    private const int AnimatedKeyCount = 64;

    public AnimatedBannerIcon(BaseReader reader)
    {
        for (var i = 0; i < AnimatedImageCount; i++)
        {
            var pixels = reader.ReadPixels<Indexed4BPP>(NDSBanner.IconWidth, NDSBanner.IconHeight);
            PixelSwizzler.UnSwizzle(ref pixels, NDSBanner.IconWidth);
            
            Images.Add(new IndexedImage(NDSBanner.IconWidth, NDSBanner.IconHeight, pixels));
        }
        
        for (var i = 0; i < AnimatedImageCount; i++)
        {
            var palette = new Palette(reader.ReadColors<BGR555>(16));
            
            Palettes.Add(palette.IsBlank ? Palettes[0] : palette);
        }
        
        for (var i = 0; i < AnimatedKeyCount; i++)
        {
            var anim = new AnimatedBannerKey(reader);
            if (anim.IsNull) break;
            
            Keys.Add(anim);
        }
    }
}

public class AnimatedBannerKey
{
    public int Duration;
    public int BitmapIndex;
    public int PaletteIndex;
    public bool FlipHorizontal;
    public bool FlipVertical;
    
    public readonly bool IsNull;
    
    private const int TickCount = (int) (1000f / 60);
    
    public AnimatedBannerKey(BaseReader reader)
    {
        var animData = reader.Read<ushort>();
        if (animData == 0x00)
        {
            IsNull = true;
            return;
        }

        Duration = (animData & 0xFF) * TickCount;
        BitmapIndex = (animData  >> 8) & 0x3;
        PaletteIndex = (animData  >> 8) & 0x3;
        FlipHorizontal = ((animData >> 14) & 0x1) != 0;
        FlipVertical = ((animData >> 15) & 0x1) != 0;

    }
}

public class LocalizedBannerTitles
{
    public string Japanese;
    public string English;
    public string French;
    public string German;
    public string Italian;
    public string Spanish;
    public string Chinese;
    public string Korean;
    
    public LocalizedBannerTitles(BaseReader reader, Version bannerVersion)
    {
        Japanese = reader.ReadString(256, true);
        English = reader.ReadString(256, true);
        French = reader.ReadString(256, true);
        German = reader.ReadString(256, true);
        Italian = reader.ReadString(256, true);
        Spanish = reader.ReadString(256, true);

        Chinese = bannerVersion.Minor > 1 ? reader.ReadString(256, true) : string.Empty;
        Korean = bannerVersion.Minor > 2 ? reader.ReadString(256, true) : string.Empty;
    }
}