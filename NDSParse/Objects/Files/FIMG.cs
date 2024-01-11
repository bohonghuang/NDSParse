using NDSParse.Data;
using NDSParse.Extensions;

namespace NDSParse.Objects.Files;

public class FIMG
{
    private const string MAGIC = "FIMG";

    public FIMG(BaseReader reader)
    {
        var magic = reader.ReadString(4).Reversed();
        if (magic != MAGIC)
        {
            throw new ParserException($"{MAGIC} has invalid magic: {magic}");
        }

        var size = reader.Read<uint>();
    } 
}