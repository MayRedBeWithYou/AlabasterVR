﻿using System;
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

    private int maxTriangleCount;

    private ComputeBuffer VertexBuffer
    {
        get
        {
            if (vertexBuffer == null) Init();
            return vertexBuffer;
        }
    }

    public float metallic;
    public float smoothness;
    public RenderType renderType;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        proceduralMaterial = Resources.Load<Material>("Materials/proceduralMaterial");
        marchingCubesShader = Resources.Load<ComputeShader>("MarchingCubes/marchingCubesGPU");
    }

    public GPUMesh(int maxTriagles)
    {
        maxTriangleCount = maxTriagles;
    }

    public void Init()
    {
        vertexBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 27, ComputeBufferType.Append);
        drawArgs = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        drawArgs.SetData(new int[] { 3, 0, 0, 0 });
    }

    public void UpdateVertexBuffer(GPUVoxelData voxels)
    {
        int kernel;
        switch (renderType)
        {
            case RenderType.Flat:
                kernel = marchingCubesShader.FindKernel("MarchingCubesFlat");
                break;
            case RenderType.Smooth:
                kernel = marchingCubesShader.FindKernel("MarchingCubesSmooth");
                break;
            default:
                throw new ArgumentException("Unknown render type");
        }
        VertexBuffer.SetCounterValue(0);
        marchingCubesShader.SetBuffer(kernel, "points", voxels.VoxelBuffer);
        marchingCubesShader.SetBuffer(kernel, "colors", voxels.ColorBuffer);
        marchingCubesShader.SetBuffer(kernel, "triangles", vertexBuffer);
        marchingCubesShader.SetInt("numPointsPerAxis", voxels.Resolution);
        marchingCubesShader.SetFloat("isoLevel", isoLevel);
        marchingCubesShader.SetFloat("chunkSize", voxels.Size);
        marchingCubesShader.Dispatch(kernel, voxels.Resolution / 8, voxels.Resolution / 8, voxels.Resolution / 8);
        ComputeBuffer.CopyCount(VertexBuffer, drawArgs, sizeof(int));
    }

    public void DrawMesh(Matrix4x4 modelMatrix, Matrix4x4 inverseModelMatrix)
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", VertexBuffer);
        materialBlock.SetMatrix("model", modelMatrix);
        materialBlock.SetMatrix("invModel", inverseModelMatrix);
        materialBlock.SetFloat("_Glossiness", smoothness);
        materialBlock.SetFloat("_Metallic", metallic);

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
        if(vertexBuffer != null) vertexBuffer.Dispose();
        if (drawArgs != null) drawArgs.Dispose();
    }
    public struct GPUTriangle
    {
        public Vector3 vertexC;
        public Vector3 vertexA;
        public Vector3 vertexB;
        public Vector3 normC;
        public Vector3 normA;
        public Vector3 normB;
        public Vector3 colorC;
        public Vector3 colorA;
        public Vector3 colorB;
    };
    public GPUTriangle[] GetTriangles()
    {
        int[] arr = new int[4];
        drawArgs.GetData(arr);
        GPUTriangle[] result = new GPUTriangle[arr[1]];
        VertexBuffer.GetData(result);
        return result;
    }
}
