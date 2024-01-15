namespace NDSParse.Objects.Files;

public class FileBase
{
    public string Path;
    public string Name => Path.Split("/").Last();
    public string Type => Path.Split(".").Last();

    public FileBase(string path)
    {
        Path = path;
    }
}