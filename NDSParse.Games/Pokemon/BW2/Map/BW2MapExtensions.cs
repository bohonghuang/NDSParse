using System.Diagnostics;
using System.Numerics;
using NDSParse.Conversion.Models;
using NDSParse.Conversion.Models.Mesh;
using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Games.Pokemon.BW2.Map;

public static class BW2MapExtensions
{
    public static bool Loaded;
    public static BW2Texts Texts;
    public static BW2MapHeader MapHeaders;
    public static List<BW2MapBuildingContainer> OutdoorBuildings = [];
    public static List<BTX0> OutdoorBuildingTilesets = [];
    public static List<BW2MapBuildingContainer> IndoorBuildings = [];
    public static List<BTX0> IndoorBuildingTilesets = [];

    public static void Init(NDSProvider provider)
    {
        if (Loaded) return;
        
        Texts = provider.LoadObject<BW2Texts>("a/0/0/2/109.bin");
        MapHeaders = provider.LoadObject<BW2MapHeader>("a/0/1/2/0.bin");
        MapHeaders.AssignNames(Texts);

        var buildingFiles = provider.Files.Values.Where(file => file.Path.Contains("a/2/2/5/"));
        foreach (var buildingFile in buildingFiles)
        {
            var buildingContainer = buildingFile.Load<BW2MapBuildingContainer>();
            OutdoorBuildings.Add(buildingContainer);
        }

        var buildingTileSetFiles = provider.Files.Values.Where(file => file.Path.Contains("a/1/7/4/"));
        foreach (var buildingTileSetFile in buildingTileSetFiles)
        {
            var buildingTileSet = buildingTileSetFile.Load<BTX0>();
            OutdoorBuildingTilesets.Add(buildingTileSet);
        }
        
        var indoorBuildingFiles = provider.Files.Values.Where(file => file.Path.Contains("a/2/2/6/"));
        foreach (var buildingFile in indoorBuildingFiles)
        {
            var buildingContainer = buildingFile.Load<BW2MapBuildingContainer>();
            IndoorBuildings.Add(buildingContainer);
        }

        var indoorBuildingTileSetFiles = provider.Files.Values.Where(file => file.Path.Contains("a/1/7/5/"));
        foreach (var buildingTileSetFile in indoorBuildingTileSetFiles)
        {
            var buildingTileSet = buildingTileSetFile.Load<BTX0>();
            IndoorBuildingTilesets.Add(buildingTileSet);
        }

        Loaded = true;
    }

    public class Actor
    {
        public Vector3 Pos;
        public Vector3 PosFlags;
        public byte Rotation;
        public Model Model;
        public Model DoorModel;
        public Vector3 DoorPos;
        public bool IsBuilding;

        public Actor(Model model, Model? doorModel = null, Vector3? doorPos = null, byte rotation = 0, Vector3? vec = null, Vector3? flag = null, bool isBuilding = false)
        {
            Model = model;
            Pos = vec ?? Vector3.Zero;
            PosFlags = flag ?? Vector3.Zero;
            Rotation = rotation;
            DoorModel = doorModel;
            DoorPos = doorPos ?? Vector3.Zero;
            IsBuilding = isBuilding;
        }
    }

    public static (List<Actor>, uint, uint, bool) GetMapModelByMapID(this NDSProvider provider, int mapIndex, uint matrix = 0, bool isOutdoor = true)
    {
        var loadedMatrix = provider.LoadObject<BW2MapMatrix>($"a/0/0/9/{matrix}.bin");
        return provider.GetMapModel((int) loadedMatrix.MapIndices.First(x => x.IsValid && x.Value == mapIndex).Index, matrix, isOutdoor);
    }

    public static (List<Actor>, uint, uint, bool) GetMapModel(this NDSProvider provider, int mapIndex, uint matrix = 0, bool isOutdoor = true)
    {
        var loadedMatrix = provider.LoadObject<BW2MapMatrix>($"a/0/0/9/{matrix}.bin");

        var actors = new List<Actor>();
        var mapFileData = loadedMatrix.MapIndices[mapIndex];
        if (!mapFileData.IsValid)
        {
            return (actors, mapFileData.X, mapFileData.Y, false);
        }

        BTX0 mapTextures;
        if (loadedMatrix.MapHeaderIndices.Count > 0)
        {
            var mapHeaderData = loadedMatrix.MapHeaderIndices[(int) mapFileData.Index];
            if (mapHeaderData.Value < MapHeaders.Headers.Count)
            {
                var mapHeader = MapHeaders.Headers[(int) mapHeaderData.Value];
                mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/{mapHeader.TextureIndex}.BTX0");
            }
            else
            {
                mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/2.BTX0");
            }

        }
        else
        {

            var otherMapHeaderData = MapHeaders.Headers.FirstOrDefault(x => x.MatrixIndex == matrix);
            if (otherMapHeaderData is not null)
            {
                mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/{otherMapHeaderData.TextureIndex}.BTX0");
            }
            else
            {
                mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/2.BTX0");
            }
        }

        var mapFile = provider.LoadObject<BW2Map>($"a/0/0/8/{mapFileData.Value}.bin");
        var mapModel = mapFile.Model.ExtractModels(mapTextures.TextureData).First();

        actors.Add(new Actor(mapModel));

        // buildings
        var buildingTarget = isOutdoor ? OutdoorBuildings : IndoorBuildings;
        var buildingTilesetTarget = isOutdoor ? OutdoorBuildingTilesets : IndoorBuildingTilesets;
        foreach (var building in mapFile.Buildings)
        {
            var foundBuildingContainer = buildingTarget.First(x => x.Headers.Any(y => y.ID == building.ModelID));
            var foundBuildingTileset = buildingTilesetTarget[buildingTarget.IndexOf(foundBuildingContainer)];
            var foundHeader = foundBuildingContainer.Headers.FirstOrDefault(x => x.ID == building.ModelID);
            if (foundHeader is null) continue;

            var buildingIndex = foundBuildingContainer.Headers.IndexOf(foundHeader);
            var buildingModel = foundBuildingContainer.Models[buildingIndex].ExtractModels(foundBuildingTileset.TextureData).First();
            var doorIndex = foundBuildingContainer.Headers.FindIndex(x => x.ID == foundHeader.DoorID && foundHeader.DoorID != -1);
            var doorModel = doorIndex == -1 ? null : foundBuildingContainer.Models[doorIndex].ExtractModels(foundBuildingTileset.TextureData).First();
            var doorPos = doorIndex == -1 ? Vector3.Zero : foundHeader.DoorLocation;
            actors.Add(new Actor(buildingModel, doorModel, doorPos, building.Rotation, building.Location, building.LocationFlags, true));
        }

        return (actors, mapFileData.X, mapFileData.Y, true);
    }
}