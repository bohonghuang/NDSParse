using NDSParse.Conversion.Textures.Images.Types;

namespace NDSParse.Conversion.Models.Mesh;

public class Material
{
    public string Name;
    public ImageTypeBase? Texture;
    public bool RepeatU;
    public bool RepeatV;
    public bool FlipU;
    public bool FlipV;
    
    
    // TODO more params from MDL0Material (diffuse, ambient, spec, emissive, transforms)
}