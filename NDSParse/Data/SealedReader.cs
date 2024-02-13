using System.Text;
using GenericReader;
using NDSParse.Objects.Files;

namespace NDSParse.Data;

public class SealedReader : BaseReader
{
    public override BaseReader AbsoluteOwner => this;

    public SealedReader(byte[] buffer, string name = "") : base(buffer, name) 
    {
        
    }
}