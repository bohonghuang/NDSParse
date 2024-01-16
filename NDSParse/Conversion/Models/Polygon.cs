using System.Numerics;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Models;

public class Polygon
{
    public PolygonType PolygonType = PolygonType.NONE;
    public List<Vector3> Vertices = [];
    public List<Vector3> Normals = [];
    public List<Vector2> TexCoords = [];
    public List<Rgba32> Colors = [];
}

public enum PolygonType
{
    NONE = -1,
    TRI = 0,
    QUAD = 1,
    TRI_STRIP = 2,
    QUAD_STRIP = 3,
}

public class Section
{
    public string Name;
    public string MaterialName;
    public List<Polygon> Polygons = [];
}