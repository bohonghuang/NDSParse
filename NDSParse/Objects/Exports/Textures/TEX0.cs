using System.Diagnostics;
using NDSParse.Conversion.Textures;
using NDSParse.Conversion.Textures.Colors;
using NDSParse.Conversion.Textures.Colors.Types;
using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Conversion.Textures.Palettes;
using NDSParse.Conversion.Textures.Pixels;
using NDSParse.Conversion.Textures.Pixels.Colored.Types;
using NDSParse.Conversion.Textures.Pixels.Indexed.Types;
using NDSParse.Data;
namespace NDSParse.Objects.Exports.Textures;

public class TEX0 : NDSExport
{
    public List<ImageTypeBase> Textures = [];

    public override string Magic => "TEX0";

    private uint TextureDataOffset;
    private ushort TextureInfoOffset;
    private uint PaletteDataOffset;
    private uint PaletteInfoOffset;
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        ReadHeader(reader);
        ReadTextures(reader);
    }

    private void ReadHeader(BaseReader reader)
    {
        reader.Position += sizeof(uint);
        reader.Position += sizeof(ushort);
        
        TextureInfoOffset = reader.Read<ushort>();
        
        reader.Position += sizeof(uint);

        TextureDataOffset = reader.Read<uint>();
        
        reader.Position += sizeof(uint);
        reader.Position += sizeof(ushort);
        reader.Position += sizeof(ushort);
        reader.Position += sizeof(uint);
        reader.Position += sizeof(uint);
        reader.Position += sizeof(uint);
        reader.Position += sizeof(uint);
        reader.Position += sizeof(uint);
        
        PaletteInfoOffset = reader.Read<uint>();
        PaletteDataOffset = reader.Read<uint>();
    }

    private void ReadTextures(BaseReader reader)
    {
        reader.Position = TextureInfoOffset;
        var textureInfos = new NameList<TEX0Info>(reader);
        
        reader.Position = PaletteInfoOffset;
        var paletteInfos = new NameListUnmanaged<ushort>(reader, () =>
        {
            var offset = reader.Read<ushort>();
            reader.Position += sizeof(ushort); // reserved
            return offset;
        });

        for (var textureIndex = 0; textureIndex < textureInfos.Count; textureIndex++)
        {
            var (textureName, textureInfo) = textureInfos.Get(textureIndex);
            var (paletteName, paletteOffset) = paletteInfos.Dict.FirstOrDefault(pair => pair.Key.Equals(textureName + "_pl"), paletteInfos.Get(textureIndex));
            
            var paletteReader = new DataBlock(reader, (int) (paletteOffset * 8 + PaletteDataOffset), textureInfo.Format.PaletteSize()).CreateReader();
            var palette = new Palette(paletteReader.ReadColors<BGR555>(paletteReader.Size / 2), paletteName);
            
            var pixelReader = new DataBlock(reader, (int) (textureInfo.TextureOffset * 8 + TextureDataOffset), textureInfo.Width * textureInfo.Height * textureInfo.Format.BitsPerPixel() / 8).CreateReader();
            var pixels = textureInfo.Format switch
            {
                TextureFormat.Color4 => pixelReader.ReadPixels<Indexed2BPP>(textureInfo.Width, textureInfo.Height),
                TextureFormat.Color16 => pixelReader.ReadPixels<Indexed4BPP>(textureInfo.Width, textureInfo.Height),
                TextureFormat.Color256 => pixelReader.ReadPixels<Indexed8BPP>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A3I5 => pixelReader.ReadPixels<A3I5>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A5I3 => pixelReader.ReadPixels<A5I3>(textureInfo.Width, textureInfo.Height),
                TextureFormat.A1BGR5 => pixelReader.ReadPixels<A1BGR555>(textureInfo.Width, textureInfo.Height)
            };

            if (textureInfo.Format.IsIndexed())
            {
                Textures.Add(new IndexedPaletteImage(pixels, [palette], textureInfo.CreateMetaData(), textureName, textureInfo.FirstColorIsTransparent));
            }
            else
            {
                Textures.Add(new ColoredImage(pixels, textureInfo.CreateMetaData(), textureName));
            }
            
        }
    }
}

public class TEX0Info : Deserializable
{
    public ushort TextureOffset;
    public TextureFormat Format;
    public int Height;
    public int Width;
    public bool FirstColorIsTransparent;
    public bool RepeatU;
    public bool RepeatV;
    public bool FlipU;
    public bool FlipV;

    public override void Deserialize(BaseReader reader)
    {
        TextureOffset = reader.Read<ushort>();

        var flags = reader.Read<ushort>();
        RepeatU = ((flags >> 0) & 1) == 1;
        RepeatV = ((flags >> 1) & 1) == 1;
        FlipU = ((flags >> 2) & 1) == 1;
        FlipV = ((flags >> 3) & 1) == 1;
        FirstColorIsTransparent = ((flags >> 13) & 1) != 0;
        Format = (TextureFormat) ((flags >> 10) & 7);
        Height = 8 << ((flags >> 7) & 7);
        Width = 8 << ((flags >> 4) & 7);

        reader.Position += 4;
    }

    public ImageMetaData CreateMetaData() => new(Width, Height, Format, RepeatU, RepeatV, FlipU, FlipV);
}