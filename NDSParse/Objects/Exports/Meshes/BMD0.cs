using NDSParse.Data;
using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Objects.Exports.Meshes;

public class BMD0 : NDSObject
{
    public MDL0 ModelData;
    public TEX0? TextureData;
    
    public override string Magic => "BMD0";
    public override bool HasBlockOffsets => true;


    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        ModelData = GetBlock<MDL0>();
        TextureData = GetBlock<TEX0>();
    }
}