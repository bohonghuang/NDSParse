using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Colored;
using NDSParse.Conversion.Textures.Pixels.Indexed;
using NDSParse.Objects.Rom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using IPixel = NDSParse.Conversion.Textures.Pixels.IPixel;

namespace NDSParse.Conversion.Textures.Images;

public static class ImageExtensions
{
    public delegate void PixelRef(ref Rgba32 pixel, int index);
    
    public static void IteratePixels(this Image<Rgba32> image, PixelRef action)
    {
        var pixelIndex = 0;
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                foreach (ref var pixel in pixelRow)
                {
                    action(ref pixel, pixelIndex);
                    pixelIndex++;
                }
            }
        });
    }
    
    public static Image<Rgba32> ToImage(this ImageTypeBase image)
    {
        return image switch
        {
            IndexedPaletteImage indexedPaletteImage => indexedPaletteImage.ToImage(),
            ColoredImage coloredImage => coloredImage.ToImage()
        };
    }
    
    public static Image<Rgba32> ToImage(this IndexedPaletteImage image)
    {
        return ToImage(image.Pixels, image.MetaData, image.Palettes, image.IsFirstColorTransparent);
    }
    
    public static Image<Rgba32> ToImage(this ColoredImage image)
    {
        return ToImage(image.Pixels, image.MetaData);
    }
    
    public static Image<Rgba32> ToImage(IPixel[] pixels, ImageMetaData metaData, List<Palette>? palettes = null, bool firstColorIsTransparent = false)
    {
        var bitmap = new Image<Rgba32>(metaData.Width, metaData.Height);
        bitmap.IteratePixels((ref Rgba32 pixel, int index) =>
        {
            var sourcePixel = pixels[index];

            Rgba32 color;
            switch (sourcePixel)
            {
                case IndexedPixel indexedPixel:
                {
                    color = palettes![indexedPixel.PaletteIndex].Colors[indexedPixel.Index].ToPixel<Rgba32>();
                    if (indexedPixel.Alpha != 255)
                    {
                        color.A = indexedPixel.Alpha;
                    }

                    if (indexedPixel.Index == 0 && firstColorIsTransparent)
                    {
                        color.A = 0;
                    }
                    
                    break;
                }
                case ColoredPixel coloredPixel:
                {
                    color = coloredPixel.Color.ToPixel<Rgba32>();
                    break;
                }
                default:
                {
                    color = new Rgba32();
                    break;
                }
            }

            pixel = color;
        });

        return bitmap;
    }
    
    public static Image<Rgba32> ToImage(this AnimatedBannerIcon icon)
    {
        var bitmap = new Image<Rgba32>(icon.Width, icon.Height);
        var rootMetaData = bitmap.Metadata.GetPngMetadata();
        rootMetaData.RepeatCount = 0;
        
        foreach (var key in icon.Keys)
        {
            var targetImage = icon.Images[key.BitmapIndex];
            var image = ToImage(targetImage.Pixels, targetImage.MetaData, [icon.Palettes[key.PaletteIndex]]);
            image.Mutate(ctx =>
            {
                if (key.FlipHorizontal) ctx.Flip(FlipMode.Horizontal);
                if (key.FlipVertical) ctx.Flip(FlipMode.Vertical);
            });
                
            var rootFrame = image.Frames.RootFrame;
            var metaData = rootFrame.Metadata.GetPngMetadata();
            metaData.FrameDelay = new Rational((uint) key.Duration, 1000);

            bitmap.Frames.AddFrame(rootFrame);
        }
        
        bitmap.Frames.RemoveFrame(0);
        return bitmap;
    }
}