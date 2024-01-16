using NDSParse.Data;
using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Conversion.Textures;

public class Texture
{
    public TEX0Info Info;
    public string TextureName;
    public string PaletteName;
    public DataBlock TextureData;
    public DataBlock PaletteData;

    public Texture(TEX0Info info, string textureName, string paletteName, DataBlock textureData, DataBlock paletteData)
    {
        Info = info;
        TextureName = textureName;
        PaletteName = paletteName;
        TextureData = textureData;
        PaletteData = paletteData;
    }
}