// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="GPUMesh.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class GPUMesh.
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="System.IDisposable" />
public class GPUMesh : IDisposable
{
    private static ComputeShader marchingCubesShader;
    private static Material proceduralMaterial;
    private static float isoLevel = 0f;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer drawArgs;

    /// <summary>
    /// The metallic
    /// </summary>
    public float metallic;
    /// <summary>
    /// The smoothness
    /// </summary>
    public float smoothness;
    /// <summary>
    /// The render type
    /// </summary>
    public RenderType renderType;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        proceduralMaterial = Resources.Load<Material>("Materials/proceduralMaterial");
        marchingCubesShader = Resources.Load<ComputeShader>("MarchingCubes/marchingCubesGPU");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GPUMesh" /> class.
    /// </summary>
    /// <param name="maxTriangleCount">The maximum triangle count.</param>
    public GPUMesh(int maxTriangleCount)
    {
        vertexBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 27, ComputeBufferType.Append);
        drawArgs = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        drawArgs.SetData(new int[] { 3, 0, 0, 0 });
    }

    /// <summary>
    /// Updates the vertex buffer.
    /// </summary>
    /// <param name="voxels">The voxels.</param>
    /// <exception cref="System.ArgumentException">Unknown render type</exception>
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
        vertexBuffer.SetCounterValue(0);
        marchingCubesShader.SetBuffer(kernel, "points", voxels.VoxelBuffer);
        marchingCubesShader.SetBuffer(kernel, "colors", voxels.ColorBuffer);
        marchingCubesShader.SetBuffer(kernel, "triangles", vertexBuffer);
        marchingCubesShader.SetInt("numPointsPerAxis", voxels.Resolution);
        marchingCubesShader.SetFloat("isoLevel", isoLevel);
        marchingCubesShader.SetFloat("chunkSize", voxels.Size);
        marchingCubesShader.Dispatch(kernel, voxels.Resolution / 8, voxels.Resolution / 8, voxels.Resolution / 8);
        ComputeBuffer.CopyCount(vertexBuffer, drawArgs, sizeof(int));
    }

    /// <summary>
    /// Draws the mesh.
    /// </summary>
    /// <param name="modelMatrix">The model matrix.</param>
    /// <param name="inverseModelMatrix">The inverse model matrix.</param>
    public void DrawMesh(Matrix4x4 modelMatrix, Matrix4x4 inverseModelMatrix)
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", vertexBuffer);
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

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        vertexBuffer.Dispose();
        drawArgs.Dispose();
    }
    /// <summary>
    /// Struct GPUTriangle
    /// </summary>
    public struct GPUTriangle
    {
        /// <summary>
        /// The vertex c
        /// </summary>
        public Vector3 vertexC;
        /// <summary>
        /// The vertex a
        /// </summary>
        public Vector3 vertexA;
        /// <summary>
        /// The vertex b
        /// </summary>
        public Vector3 vertexB;
        /// <summary>
        /// The norm c
        /// </summary>
        public Vector3 normC;
        /// <summary>
        /// The norm a
        /// </summary>
        public Vector3 normA;
        /// <summary>
        /// The norm b
        /// </summary>
        public Vector3 normB;
        /// <summary>
        /// The color c
        /// </summary>
        public Vector3 colorC;
        /// <summary>
        /// The color a
        /// </summary>
        public Vector3 colorA;
        /// <summary>
        /// The color b
        /// </summary>
        public Vector3 colorB;
    };
    /// <summary>
    /// Gets the triangles.
    /// </summary>
    /// <returns>GPUTriangle[].</returns>
    public GPUTriangle[] GetTriangles()
    {
        int[] arr = new int[4];
        drawArgs.GetData(arr);
        GPUTriangle[] result = new GPUTriangle[arr[1]];
        vertexBuffer.GetData(result);
        return result;
    }
}
