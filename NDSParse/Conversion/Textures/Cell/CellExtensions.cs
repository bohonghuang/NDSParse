using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Objects.Exports.Palettes;
using NDSParse.Objects.Exports.Textures;
using NDSParse.Objects.Exports.Textures.Cell;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Textures.Cell;

public static class CellExtensions
{
    public static List<IndexedPaletteImage> ExtractCells(this RECN resource, RGCN texture, RLCN palette, bool firstColorIsTransparent = true)
    {
        var image = texture.CombineWithPalette(palette, firstColorIsTransparent: firstColorIsTransparent);
        var cells = resource.CellBank.Banks.SelectMany(bank => bank.Cells);

        var outputImages = new List<IndexedPaletteImage>();
        foreach (var cell in cells)
        {
            var tileOffset = cell.TileOffset << resource.CellBank.BlockSize;
            var startByte = tileOffset * 32;
            var startPixel = startByte * (8 / image.MetaData.Format.BitsPerPixel());

            var pixels = image.Pixels.Skip(startPixel).Take(cell.Width * cell.Height).ToArray();
            outputImages.Add(new IndexedPaletteImage(pixels, image.Palettes, new ImageMetaData(cell.Width, cell.Height, image.MetaData.Format), isFirstColorTransparent: firstColorIsTransparent));
        }

        return outputImages;
    }
}