using System.Numerics;
using NDSParse.Objects.Exports.Meshes;

namespace NDSParse.Conversion.Models;

public class SimulatedGPU
{
    public Matrix4x4[] MatrixStack;
    public Matrix4x4 CurrentMatrix = Matrix4x4.Identity;
    public MDL0Material CurrentMaterial;
    
    public SimulatedGPU()
    {
        MatrixStack = new Matrix4x4[32];
        for (var i = 0; i < MatrixStack.Length; i++)
        {
            MatrixStack[i] = Matrix4x4.Identity;
        }
    }
    
    public void Restore(int stackIndex)
    {
        CurrentMatrix = MatrixStack[stackIndex];
    }

    public void Store(int stackIndex)
    {
        MatrixStack[stackIndex] = CurrentMatrix;
    }

    public void Mult(Matrix4x4 matrix)
    {
        CurrentMatrix = matrix * CurrentMatrix;
    }
}