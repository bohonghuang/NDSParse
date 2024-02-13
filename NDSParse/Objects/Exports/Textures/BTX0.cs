using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Data;

namespace NDSParse.Objects.Exports.Textures;

public class BTX0 : NDSObject
{
    public TEX0 TextureData;
    
    public override string Magic => "BTX0";
    public override bool HasBlockOffsets => true;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        TextureData = GetBlock<TEX0>();
    }
}