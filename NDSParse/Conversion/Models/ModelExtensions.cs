using NDSParse.Conversion.Models.Formats;
using NDSParse.Conversion.Models.Mesh;
using NDSParse.Conversion.Models.Processing;
using NDSParse.Objects.Exports.Meshes;

namespace NDSParse.Conversion.Models;

public static class ModelExtensions
{
    public static List<Model> ExtractModels(this BMD0 bmd0)
    {
        var models = new List<Model>();
        foreach (var dataModel in bmd0.ModelData.Models)
        {
            var processor = new MeshProcessor(dataModel);
            
            var model = new Model();
            model.Name = dataModel.Name;
            model.Sections = processor.Process();

            var vertexIndex = 0;
            foreach (var section in model.Sections)
            {
                foreach (var polygon in section.Polygons)
                {
                    switch (polygon.PolygonType)
                    {
                        case PolygonType.TRI:
                        {
                            for (var vtxCounter = 0; vtxCounter < polygon.Vertices.Count; vtxCounter += 3)
                            {
                                var face = new Face(section.MaterialName);
                                for (var vtxIdx = 0; vtxIdx < 3; vtxIdx++)
                                {
                                    face.AddIndex(vertexIndex + vtxIdx);
                                }
                                section.Faces.Add(face);
                            }

                            vertexIndex += polygon.Vertices.Count;
                            break;
                        }
                        case PolygonType.QUAD:
                        {
                            for (var vtxCounter = 0; vtxCounter < polygon.Vertices.Count; vtxCounter += 4)
                            {
                                var face = new Face(section.MaterialName);
                                for (var vtxIdx = 0; vtxIdx < 4; vtxIdx++)
                                {
                                    face.AddIndex(vertexIndex + vtxIdx);
                                }
                                section.Faces.Add(face);
                            }
                            
                            vertexIndex += polygon.Vertices.Count;
                            break;
                        }
                        case PolygonType.TRI_STRIP:
                        case PolygonType.QUAD_STRIP:
                        {
                            for (var vtxCounter = 0; vtxCounter + 2 < polygon.Vertices.Count; vtxCounter += 2)
                            {
                                var firstFace = new Face(section.MaterialName);
                                firstFace.AddIndex(vertexIndex + vtxCounter);
                                firstFace.AddIndex(vertexIndex + vtxCounter + 1);
                                firstFace.AddIndex(vertexIndex + vtxCounter + 2);
                                section.Faces.Add(firstFace);

                                if (vtxCounter + 3 < polygon.Vertices.Count)
                                {
                                    var extraFace = new Face(section.MaterialName);
                                    extraFace.AddIndex(vertexIndex + vtxCounter + 1);
                                    extraFace.AddIndex(vertexIndex + vtxCounter + 3);
                                    extraFace.AddIndex(vertexIndex + vtxCounter + 2);
                                    section.Faces.Add(extraFace);
                                }
                                
                            }
                            
                            vertexIndex += polygon.Vertices.Count;
                            break;
                        }
                    }
                }
            }

            foreach (var dataMaterial in dataModel.Materials)
            {
                var material = new Material
                {
                    Name = dataMaterial.Name,
                    Texture = bmd0.TextureData?.Textures.FirstOrDefault(texture => texture.Name.Equals(dataMaterial.TextureName, StringComparison.OrdinalIgnoreCase))
                };
                model.Materials.Add(material);
            }
            models.Add(model);
        }
        return models;
    }

    public static OBJ ToOBJ(this Model model)
    {
        return new OBJ(model);
    }
}
