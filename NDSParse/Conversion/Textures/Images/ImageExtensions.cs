using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Objects.Rom;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace NDSParse.Conversion.Textures.Images;

public static class ImageExtensions
{
    public static Image<Rgba32> ToImage(this IndexedPaletteImage image)
    {
        return ToImage(image.Width, image.Height, image.Pixels, image.Palettes);
    }
    
    public static Image<Rgba32> ToImage(int width, int height, IndexedPixel[] pixels, List<Palette> palettes)
    {
        var bitmap = new Image<Rgba32>(width, height);
        var pixelIndex = 0;
        bitmap.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                foreach (ref var pixel in pixelRow)
                {
                    var sourcePixel = pixels[pixelIndex];
                    pixel = palettes[sourcePixel.PaletteIndex].Colors[sourcePixel.Index].ToPixel<Rgba32>();
                    pixelIndex++;
                }
            }
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