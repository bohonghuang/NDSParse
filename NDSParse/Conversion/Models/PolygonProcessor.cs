using System.Numerics;
using NDSParse.Extensions;
using NDSParse.Objects.Exports.Meshes;
using SixLabors.ImageSharp.PixelFormats;

namespace NDSParse.Conversion.Models;

public class PolygonProcessor
{
    public Section Section = new();
    private MDL0Polygon Polygon;
    private SimulatedGPU GPU;
    
    public PolygonProcessor(MDL0Polygon polygon, SimulatedGPU gpu)
    {
        Polygon = polygon;
        GPU = gpu;
    }

    public Section Process()
    {
        Polygon.Commands.ForEach(ProcessCommand);
        return Section;
    }
    
    public PolygonType CurrentPolygonType;
    public Vector3 CurrentVertex;
    public Vector3 CurrentNormal;
    public Vector2 CurrentTexCoord;
    public Rgba32 CurrentColor;

    public List<Vector3> Vertices = [];
    public List<Vector3> Normals = [];
    public List<Vector2> TexCoords = [];
    public List<Rgba32> Colors = [];
    
    public void PushVertex()
    {
        Vertices.Add(Vector3.Transform(CurrentVertex, GPU.CurrentMatrix));
        Normals.Add(CurrentNormal);
        TexCoords.Add(CurrentTexCoord);
        Colors.Add(CurrentColor);
    }

    public void ProcessCommand(MDL0PolygonCommand command)
    {
        switch (command.OpCode)
        {
            case PolygonCommandOpCode.MTX_STORE:
                GPU.Store(CommandProcessor.MTX_STORE(command));
                break;
            case PolygonCommandOpCode.MTX_RESTORE:
                GPU.Store(CommandProcessor.MTX_RESTORE(command) & 31);
                break;
            case PolygonCommandOpCode.MTX_SCALE:
                GPU.Mult(Matrix4x4.CreateScale(CommandProcessor.MTX_SCALE(command)));
                break;
            case PolygonCommandOpCode.MTX_TRANS:
                GPU.Mult(Matrix4x4.CreateTranslation(CommandProcessor.MTX_TRANS(command)));
                break;
            case PolygonCommandOpCode.COLOR:
                CurrentColor = CommandProcessor.COLOR(command);
                break;
            case PolygonCommandOpCode.NORMAL:
                CurrentNormal = Vector3.Normalize(Vector3.TransformNormal(CommandProcessor.NORMAL(command), GPU.CurrentMatrix));
                break;
            case PolygonCommandOpCode.TEXCOORD:
                CurrentTexCoord = CommandProcessor.TEXCOORD(command);
                CurrentTexCoord.X = CurrentTexCoord.X / GPU.CurrentMaterial.Width;
                CurrentTexCoord.Y = -CurrentTexCoord.Y / GPU.CurrentMaterial.Height;
                break;
            case PolygonCommandOpCode.VTX_16:
                CurrentVertex = CommandProcessor.VTX_16(command);
                PushVertex();
                break;
            case PolygonCommandOpCode.VTX_10:
                CurrentVertex = CommandProcessor.VTX_10(command);
                PushVertex();
                break;
            case PolygonCommandOpCode.VTX_XY:
                var xy = CommandProcessor.VTX_XY(command);
                CurrentVertex.X = xy.X;
                CurrentVertex.Y = xy.Y;
                PushVertex();
                break;
            case PolygonCommandOpCode.VTX_XZ:
                var xz = CommandProcessor.VTX_XZ(command);
                CurrentVertex.X = xz.X;
                CurrentVertex.Z = xz.Z;
                PushVertex();
                break;
            case PolygonCommandOpCode.VTX_YZ:
                var yz = CommandProcessor.VTX_YZ(command);
                CurrentVertex.Y = yz.Y;
                CurrentVertex.Z = yz.Z;
                PushVertex();
                break;
            case PolygonCommandOpCode.VTX_DIFF:
                CurrentVertex += CommandProcessor.VTX_DIFF(command) / 8;
                PushVertex();
                break;
            case PolygonCommandOpCode.BEGIN_VTXS:
                CurrentPolygonType = CommandProcessor.BEGIN_VTXS(command);
                break;
            case PolygonCommandOpCode.END_VTXS:
                Section.Polygons.Add(new Polygon
                {
                    Vertices = Vertices.ToList(),
                    Normals = Normals.ToList(),
                    TexCoords = TexCoords.ToList(),
                    Colors = Colors.ToList(),
                    PolygonType = CurrentPolygonType
                });
                
                Vertices.Clear();
                Normals.Clear();
                TexCoords.Clear();
                Colors.Clear();
                break;
        }
    }
}

public static class CommandProcessor
{
    public static int MTX_STORE(MDL0PolygonCommand command)
    {
        return command.Parameters[0];
    }
    
    public static int MTX_RESTORE(MDL0PolygonCommand command)
    {
        return command.Parameters[0];
    }
    
    public static Vector3 MTX_SCALE(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0], 1, 19, 12),
            FloatExtensions.ToFloat(command.Parameters[1], 1, 19, 12),
            FloatExtensions.ToFloat(command.Parameters[2], 1, 19, 12));
    }

    public static Vector3 MTX_TRANS(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0], 1, 19, 12),
            FloatExtensions.ToFloat(command.Parameters[1], 1, 19, 12),
            FloatExtensions.ToFloat(command.Parameters[2], 1, 19, 12));
    }

    public static Vector3 NORMAL(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0] & 0x3FF, 1, 0, 9),
            FloatExtensions.ToFloat((command.Parameters[0] >> 10) & 0x3FF, 1, 0, 9),
            FloatExtensions.ToFloat((command.Parameters[0] >> 20) & 0x3FF, 1, 0, 9));
    }
    
    public static Rgba32 COLOR(MDL0PolygonCommand command)
    {
        var r = command.Parameters[0] & 0x1F;
        var g = (command.Parameters[0] >> 5) & 0x1F;
        var b = (command.Parameters[0] >> 10) & 0x1F;
        return new Rgba32(r / 31.0f, g / 31.0f, b / 31.0f);
    }
    
    public static Vector2 TEXCOORD(MDL0PolygonCommand command)
    {
        return new Vector2(
            FloatExtensions.ToFloat(command.Parameters[0] & 0xFFFF, 1, 11, 4),
            FloatExtensions.ToFloat(command.Parameters[0] >> 16, 1, 11, 4));
    }

    public static Vector3 VTX_16(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0] & 0xFFFF, 1, 3, 12),
            FloatExtensions.ToFloat(command.Parameters[0] >> 16, 1, 3, 12),
            FloatExtensions.ToFloat(command.Parameters[1] & 0xFFFF, 1, 3, 12));
    }

    public static Vector3 VTX_10(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0] & 0x3FF, 1, 3, 6),
            FloatExtensions.ToFloat((command.Parameters[0] >> 10) & 0x3FF, 1, 3, 6),
            FloatExtensions.ToFloat((command.Parameters[0] >> 20) & 0x3FF, 1, 3, 6));
    }

    public static Vector3 VTX_XY(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0] & 0xFFFF, 1, 3, 12),
            FloatExtensions.ToFloat(command.Parameters[0] >> 16, 1, 3, 12),
            0);
    }

    public static Vector3 VTX_XZ(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0] & 0xFFFF, 1, 3, 12),
            0,
            FloatExtensions.ToFloat(command.Parameters[0] >> 16, 1, 3, 12));
    }

    public static Vector3 VTX_YZ(MDL0PolygonCommand command)
    {
        return new Vector3(
            0,
            FloatExtensions.ToFloat(command.Parameters[0] & 0xFFFF, 1, 3, 12),
            FloatExtensions.ToFloat(command.Parameters[0] >> 16, 1, 3, 12));
    }

    public static Vector3 VTX_DIFF(MDL0PolygonCommand command)
    {
        return new Vector3(
            FloatExtensions.ToFloat(command.Parameters[0] & 0x3FF, 1, 0, 9),
            FloatExtensions.ToFloat((command.Parameters[0] >> 10) & 0x3FF, 1, 0, 9),
            FloatExtensions.ToFloat((command.Parameters[0] >> 20) & 0x3FF, 1, 0, 9));
    }
    
    public static PolygonType BEGIN_VTXS(MDL0PolygonCommand command)
    {
        return (PolygonType) command.Parameters[0];
    }
}
