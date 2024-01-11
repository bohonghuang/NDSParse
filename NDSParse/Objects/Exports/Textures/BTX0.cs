using NDSParse.Data;

namespace NDSParse.Objects.Exports.Textures;

public class BTX0 : NDSObject
{
    public TEX0 Texture;
    
    public override string Magic => "BTX0";
    protected override bool ContainsSubfiles => true;

    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        if (SubFileCount > 1)
        {
            throw new ParserException("BTX0 should not have more than on sub-file.");
        }

        Texture = Construct<TEX0>(reader.Spliced(SubFileOffsets[0]));
    }
}