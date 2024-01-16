using System.Numerics;
using NDSParse.Conversion.Models.Mesh;
using NDSParse.Objects.Exports.Meshes;

namespace NDSParse.Conversion.Models.Processing;

public class MeshProcessor
{
    public List<Section> Sections = [];
    
    private MDL0Model Model;
    private SimulatedGPU GPU = new();
    
    public MeshProcessor(MDL0Model model)
    {
        Model = model;
        GPU.CurrentMaterial = model.Materials[0];
    }

    public List<Section> Process()
    {
        Model.RenderCommands.ForEach(ProcessCommand);
        return Sections;
    }

    public void ProcessCommand(MDL0RenderCommand command)
    {
         switch (command.OpCode)
        {
            case RenderCommandOpCode.MTX_RESTORE:
                GPU.Restore(command.Parameters[0]);
                break;

            case RenderCommandOpCode.BIND_MATERIAL:
                GPU.CurrentMaterial = Model.Materials[command.Parameters[0]];
                break;

            case RenderCommandOpCode.DRAW_MESH:
                var polygon = Model.Polygons[command.Parameters[0]];
                var processor = new PolygonProcessor(polygon, GPU);
                
                var section = processor.Process();
                section.Name = polygon.Name;
                section.MaterialName = GPU.CurrentMaterial.Name;
                Sections.Add(section);
                break;

            case RenderCommandOpCode.MTX_MULT:
                var meshInfoIndex = command.Parameters[0];

                var restoreIndex = -1;
                var storeIndex = -1;
                switch (command.Flags)
                {
                    case 0: // 3 params
                        break;
                    case 1: // 4 params
                        storeIndex = command.Parameters[3];
                        break;
                    case 2: // 4 params
                        restoreIndex = command.Parameters[3];
                        break;
                    case 3: // 5 params
                        storeIndex = command.Parameters[3];
                        restoreIndex = command.Parameters[4];
                        break;
                }

                if (restoreIndex != -1)
                {
                    GPU.Restore(restoreIndex);
                }

                var data = Model.MeshInfos[meshInfoIndex];
                GPU.Mult(data.Matrix);

                if (storeIndex != -1)
                {
                    GPU.Store(storeIndex);
                }

                break;

            case RenderCommandOpCode.MTX_SCALE:
                GPU.Mult(command.Flags == 1 ? Matrix4x4.CreateScale(Model.DownScale) : Matrix4x4.CreateScale(Model.UpScale));
                break;
        }
    }
}