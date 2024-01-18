using NDSParse.Conversion.Models;
using NDSParse.Conversion.Models.Mesh;
using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Games.Pokemon.BW2.Map;

public static class BW2MapExtensions
{
    // TODO REMOVE ---- FOR TESTING ----
    // TODO BUILDING DATA
    public static Model GetMapModel(this NDSProvider provider, string name, uint matrix = 0)
    {
        var mapTexts = provider.LoadObject<BW2Texts>("a/0/0/2/109.bin");
        var mapHeaders = provider.LoadObject<BW2MapHeader>("a/0/1/2/0.bin");
        mapHeaders.AssignNames(mapTexts);

        var mapMatrix = provider.LoadObject<BW2MapMatrix>($"a/0/0/9/{matrix}.bin");
        var mapHeaderIndex = mapMatrix.MapHeaderIndices.First(index => index.IsValid && mapHeaders.Headers[(int) index.Value].Name.Equals(name));
        var mapHeader = mapHeaders.Headers[(int) mapHeaderIndex.Value];
        var mapFileIndex = mapMatrix.MapIndices[mapMatrix.MapHeaderIndices.IndexOf(mapHeaderIndex)];
        var mapFile = provider.LoadObject<BW2Map>($"a/0/0/8/{mapFileIndex.Value}.bin");
        var mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/{mapHeader.TextureIndex}.btx0");
        var mapModel = mapFile.Model.ExtractModels(mapTextures.TextureData).First();
        return mapModel;
    }
}