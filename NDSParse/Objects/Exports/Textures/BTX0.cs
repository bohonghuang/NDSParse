using NDSParse.Conversion.Textures.Images.Types;
using NDSParse.Data;

namespace NDSParse.Objects.Exports.Textures;

public class BTX0 : NDSObject
{
    public TEX0 TextureData;
    
    public override string Magic => "BTX0";
    protected override bool ContainsSubfiles => true;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        ManageSubFiles(reader, (ext, offset) =>
        {
            TextureData = ext switch
            {
                "TEX0" => ConstructExport<TEX0>(reader.Spliced(offset))
            };
        });
    }

    public IEnumerable<ImageTypeBase> GetImages() => TextureData.Textures;
}