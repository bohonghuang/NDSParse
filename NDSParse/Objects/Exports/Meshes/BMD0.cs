using NDSParse.Data;
using NDSParse.Objects.Exports.Textures;

namespace NDSParse.Objects.Exports.Meshes;

public class BMD0 : NDSObject
{
    public MDL0 Model;
    public TEX0 TextureData;
    
    public override string Magic => "BMD0";

    protected override bool ContainsSubfiles => true;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        ManageSubFiles(reader, (ext, offset) =>
        {
            switch (ext)
            {
                case "MDL0":
                    Model = Construct<MDL0>(reader.Spliced(offset));
                    break;
                case "TEX0":
                    TextureData = Construct<TEX0>(reader.Spliced(offset));
                    break;
            }
        });
    }
}