using NDSParse.Data;
using NDSParse.Objects.Exports.Sounds.SoundData;

namespace NDSParse.Objects.Exports.Sounds;

public abstract class SoundTypeBase<T> : NDSObject where T : SoundInfoTypeBase
{
    public T Info;
}

public class SoundInfoTypeBase : Deserializable
{
    public ushort FileID;
    
    public override void Deserialize(BaseReader reader)
    {
        FileID = reader.Read<ushort>();
    }
}