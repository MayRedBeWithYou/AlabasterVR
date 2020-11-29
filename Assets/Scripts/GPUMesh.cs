using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUMesh : IDisposable
{
    private static ComputeShader marchingCubesShader;
    private static Material proceduralMaterial;
    private static float isoLevel = 0f;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer drawArgs;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        proceduralMaterial = Resources.Load<Material>("MarchingCubes/proceduralMaterial");
        marchingCubesShader = Resources.Load<ComputeShader>("MarchingCubes/marchingCubesGPU");
    }
    public GPUMesh(int maxTriangleCount)
    {
        vertexBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 18, ComputeBufferType.Append);
        drawArgs = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        drawArgs.SetData(new int[] { 3, 0, 0, 0 });
    }

    public void UpdateVertexBuffer(GPUVoxelData voxels)
    {
        int kernel = marchingCubesShader.FindKernel("March");
        vertexBuffer.SetCounterValue(0);
        marchingCubesShader.SetBuffer(kernel, "points", voxels.VoxelBuffer);
        marchingCubesShader.SetBuffer(kernel, "triangles", vertexBuffer);
        marchingCubesShader.SetInt("numPointsPerAxis", voxels.Resolution);
        marchingCubesShader.SetFloat("isoLevel", isoLevel);
        marchingCubesShader.SetFloat("chunkSize", voxels.Size);
        marchingCubesShader.Dispatch(kernel, voxels.Resolution / 8, voxels.Resolution / 8, voxels.Resolution / 8);
        ComputeBuffer.CopyCount(vertexBuffer, drawArgs, sizeof(int));
        var array = new float[vertexBuffer.count * 72 / 4];
        vertexBuffer.GetData(array);
        for (int i = 0; i < 12; i++)
        {
            Debug.Log($"{array[i]}");
        }

        var draw = new int[4];
        drawArgs.GetData(draw);
        Debug.Log($"Triangles: {draw[1]}");

    }

    public void DrawMesh(Vector3 offset)
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", vertexBuffer);
        materialBlock.SetVector("offset", offset);
        Graphics.DrawProceduralIndirect(
            proceduralMaterial,
            new Bounds(Vector3.zero, new Vector3(100, 100, 100)), //what exactly should go here?
            MeshTopology.Triangles,
            drawArgs,
            0,
            null,
            materialBlock
            );
    }

    public void Dispose()
    {
        vertexBuffer.Dispose();
        drawArgs.Dispose();
    }
}
