using NDSParse.Data;
using NDSParse.Objects.Rom;

namespace NDSParse;

public class NDSProvider
{
    public NDSHeader Header;
    public NDSBanner Banner;
    
    private BaseReader _reader;
    private FileInfo _file;

    public NDSProvider(FileInfo file)
    {
        _file = file;
    }
    
    public NDSProvider(string path) : this(new FileInfo(path)) { }

    public void Initialize()
    {
        _reader = new BaseReader(File.ReadAllBytes(_file.FullName), _file.Name);
        
        Header = new NDSHeader(_reader);
        Banner = new NDSBanner(Header.BannerData.CreateReader());
    }
}