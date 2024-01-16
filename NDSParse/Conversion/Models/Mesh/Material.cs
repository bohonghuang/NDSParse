using NDSParse.Conversion.Textures.Images.Types;

namespace NDSParse.Conversion.Models.Mesh;

public class Material
{
    public string Name;
    public ImageTypeBase? Texture;
    // TODO more params from MDL0Material (diffuse, ambient, spec, emissive, transforms)
}