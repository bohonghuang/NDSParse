using NDSParse.Conversion.Textures.Colors;
using NDSParse.Conversion.Textures.Colors.Types;
using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Types;
using NDSParse.Objects.Rom;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
    
    public static Image<Rgba32> ToImage(this IndexedPaletteImage image)
    {
        return ToImage(image.Width, image.Height, image.Pixels, image.Palettes, image.IsFirstColorTransparent);
    }
    
    public static Image<Rgba32> ToImage(int width, int height, IndexedPixel[] pixels, List<Palette> palettes, bool firstColorIsTransparent = false)
    {
        var bitmap = new Image<Rgba32>(width, height);
        bitmap.IteratePixels((ref Rgba32 pixel, int index) =>
        {
            var sourcePixel = pixels[index];
            var color = palettes[sourcePixel.PaletteIndex].Colors[sourcePixel.Index].ToPixel<Rgba32>();
            if (sourcePixel.Alpha != 255)
            {
                color.A = sourcePixel.Alpha;
            }

            if (sourcePixel.Index == 0 && firstColorIsTransparent)
            {
                color.A = 0;
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
            var image = ToImage(targetImage.Width, targetImage.Height, targetImage.Pixels, [icon.Palettes[key.PaletteIndex]]);
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