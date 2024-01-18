using NDSParse.Data;
using NDSParse.Objects;
using NDSParse.Objects.Exports.Meshes;

namespace NDSParse.Games.Pokemon.BW2.Map;

public class BW2Map : Deserializable
{
    public BMD0 Model;
    
    public override void Deserialize(BaseReader reader)
    {
        reader.Position += 20;

        Model = Construct<BMD0>(reader.Spliced());
    }
}