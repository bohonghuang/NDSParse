using NDSParse.Data;
using NDSParse.Objects.Exports.Palettes;

namespace NDSParse.Objects.Exports.Textures;

public class RGCN : NDSObject
{
    public RAHC TextureData;
    
    public override string Magic => "RGCN";

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        TextureData = GetBlock<RAHC>();
    }
}