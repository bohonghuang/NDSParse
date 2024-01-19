using System.Numerics;
using NDSParse.Conversion.Models;
using NDSParse.Conversion.Models.Mesh;
using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Games.Pokemon.BW2.Map;

public static class BW2MapExtensions
{
    // TODO REMOVE ---- FOR TESTING ----
    public static List<Actor> GetMapModel(this NDSProvider provider, string name, uint matrix = 0)
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

        var actors = new List<Actor>();
        actors.Add(new Actor(mapModel));
        
        // buildings
        var buildingContainer = provider.LoadObject<BW2MapBuildingContainer>("a/2/2/5/0.bin");
        var buildingTileset = provider.LoadObject<BTX0>("a/1/7/4/0.btx0");
        foreach (var building in mapFile.Buildings)
        {
            var buildingIndex = buildingContainer.Headers.FindIndex(x => x.ID == building.ModelID);
            var buildingModel = buildingContainer.Models[buildingIndex].ExtractModels(buildingTileset.TextureData).First();
            actors.Add(new Actor(buildingModel, building.Location));
        }
        
        
        return actors;
    }

    public class Actor
    {
        public Vector3 Pos; 
        public Model Model;

        public Actor(Model model, Vector3? vec = null)
        {
            Model = model;
            Pos = vec ?? Vector3.Zero;
        }
    }
}