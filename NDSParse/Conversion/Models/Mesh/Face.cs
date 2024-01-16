namespace NDSParse.Conversion.Models.Mesh;

public class Face
{
    public string MaterialName;
    public List<int> VertexIndices = [];
    public List<int> NormalIndices = [];
    public List<int> TexCoordIndices = [];
    public List<int> ColorIndices = [];

    public Face(string materialName)
    {
        MaterialName = materialName;
    }

    public void AddIndex(int index)
    {
        VertexIndices.Add(index);
        NormalIndices.Add(index);
        TexCoordIndices.Add(index);
        ColorIndices.Add(index);
    }
}