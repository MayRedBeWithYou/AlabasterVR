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
        proceduralMaterial = Resources.Load<Material>("Materials/proceduralMaterial");
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
    }

    public void DrawMesh(Matrix4x4 modelMatrix, Matrix4x4 inverseModelMatrix)
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", vertexBuffer);
        materialBlock.SetMatrix("model", modelMatrix);
        materialBlock.SetMatrix("invModel", inverseModelMatrix);

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
    public struct Triangle
    {
        public Vector3 vertexC;
        public Vector3 vertexA;
        public Vector3 vertexB;
        public Vector3 normC;
        public Vector3 normB;
        public Vector3 normA;
    };
    public Triangle[] GetTriangles()
    {
        ComputeBuffer buffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        ComputeBuffer.CopyCount(vertexBuffer, buffer, 0);
        int[] arr = new int[1];
        buffer.GetData(arr);
        Triangle[] result = new Triangle[arr[0]];
        vertexBuffer.GetData(result);
        return result;
    }
}
