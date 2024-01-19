using System.Numerics;
using NDSParse.Conversion.Models;
using NDSParse.Conversion.Models.Mesh;
using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Games.Pokemon.BW2.Map;

public static class BW2MapExtensions
    {
        public static BW2Texts Texts;
        public static BW2MapHeader MapHeaders;
        public static BW2MapMatrix OverworldMatrix;
        public static List<BW2MapBuildingContainer> Buildings = [];
        public static List<BTX0> BuildingTilesets = [];
        
        public static void Init(NDSProvider provider)
        {
            Texts = provider.LoadObject<BW2Texts>("a/0/0/2/109.bin");
            MapHeaders = provider.LoadObject<BW2MapHeader>("a/0/1/2/0.bin");
            MapHeaders.AssignNames(Texts);
            OverworldMatrix = provider.LoadObject<BW2MapMatrix>($"a/0/0/9/0.bin");

            var buildingFiles = provider.Files.Values.Where(file => file.Path.Contains("a/2/2/5/"));
            foreach (var buildingFile in buildingFiles)
            {
                var buildingContainer = buildingFile.Load<BW2MapBuildingContainer>();
                Buildings.Add(buildingContainer);
            }
            
            var buildingTileSetFiles = provider.Files.Values.Where(file => file.Path.Contains("a/1/7/4/"));
            foreach (var buildingTileSetFile in buildingTileSetFiles)
            {
                var buildingTileSet = buildingTileSetFile.Load<BTX0>();
                BuildingTilesets.Add(buildingTileSet);
            }
        }
        
        public class Actor
        {
            public Vector3 Pos; 
            public Vector3 PosFlags; 
            public byte Rotation; 
            public Model Model;
            public Model DoorModel;
            public Vector3 DoorPos; 

            public Actor(Model model, Model? doorModel = null, Vector3? doorPos = null, byte rotation = 0, Vector3? vec = null, Vector3? flag =null)
            {
                Model = model;
                Pos = vec ?? Vector3.Zero;
                PosFlags = flag ?? Vector3.Zero;
                Rotation = rotation;
                DoorModel = doorModel;
                DoorPos = doorPos ?? Vector3.Zero;
            }
        }
        
        public static (List<Actor>, uint, uint, bool) GetMapModel(this NDSProvider provider, int mapIndex, uint matrix = 0)
        {
            var actors = new List<Actor>();
            var mapFileData = OverworldMatrix.MapIndices[mapIndex];
            if (!mapFileData.IsValid)
            {
                return (actors, mapFileData.X, mapFileData.Y, false);
            }
            BTX0 mapTextures;
            if (OverworldMatrix.MapHeaderIndices.Count > 0)
            {
                var mapHeaderData = OverworldMatrix.MapHeaderIndices[(int) mapFileData.Index];
                if (!mapHeaderData.IsValid)
                {
                    mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/2.btx0");
                }
                else
                {
                    var mapHeader = MapHeaders.Headers[(int) mapHeaderData.Value];
                    mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/{mapHeader.TextureIndex}.btx0");
                }
            }
            else
            {
                mapTextures = provider.LoadObject<BTX0>($"a/0/1/4/2.btx0");
            }
           
            var mapFile = provider.LoadObject<BW2Map>($"a/0/0/8/{mapFileData.Value}.bin");
            var mapModel = mapFile.Model.ExtractModels(mapTextures.TextureData).First();
            
            actors.Add(new Actor(mapModel));
        
            // buildings
            foreach (var building in mapFile.Buildings)
            {
                var foundBuildingContainer = Buildings.First(x => x.Headers.Any(y => y.ID == building.ModelID));
                var foundBuildingTileset = BuildingTilesets[Buildings.IndexOf(foundBuildingContainer)];
                var foundHeader = foundBuildingContainer.Headers.FirstOrDefault(x => x.ID == building.ModelID);
                if (foundHeader is null) continue;
                
                var buildingIndex = foundBuildingContainer.Headers.IndexOf(foundHeader);
                var buildingModel = foundBuildingContainer.Models[buildingIndex].ExtractModels(foundBuildingTileset.TextureData).First();
                var doorIndex = foundBuildingContainer.Headers.FindIndex(x => x.ID == foundHeader.DoorID && foundHeader.DoorID != -1);
                var doorModel = doorIndex == -1 ? null : foundBuildingContainer.Models[doorIndex].ExtractModels(foundBuildingTileset.TextureData).First();
                var doorPos = doorIndex == -1 ? Vector3.Zero : foundHeader.DoorLocation;
                actors.Add(new Actor(buildingModel, doorModel, doorPos, building.Rotation, building.Location, building.LocationFlags));
            }
        
            return (actors, mapFileData.X, mapFileData.Y, true);
        }
    }