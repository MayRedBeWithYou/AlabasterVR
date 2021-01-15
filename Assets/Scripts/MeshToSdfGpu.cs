// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="MeshToSdfGpu.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
/// <summary>
/// Class MeshToSdfGpu.
/// </summary>
public static class MeshToSdfGpu
{
    /// <summary>
    /// Class TemporaryMesh.
    /// </summary>
    public class TemporaryMesh
    {
        /// <summary>
        /// The triangles
        /// </summary>
        public float[] triangles;
    }

    private static ComputeShader meshToSdfShader;
    private static ComputeBuffer trianglesBuffer;
    /// <summary>
    /// The bounds
    /// </summary>
    public static Bounds bounds;
    private static void Initialize(TemporaryMesh m)
    {
        trianglesBuffer = new ComputeBuffer(m.triangles.Length, sizeof(float));
        trianglesBuffer.SetData(m.triangles);
    }
    /// <summary>
    /// Translates the triangles to SDF.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <param name="trianglesCount">The triangles count.</param>
    /// <param name="calculateOnGpu">if set to <c>true</c> [calculate on gpu].</param>
    public static void TranslateTrianglesToSdf(TemporaryMesh m, int trianglesCount, bool calculateOnGpu = true)
    {
        meshToSdfShader = Resources.Load<ComputeShader>("MeshToSdf/MeshToSdfNormalsShader");
        Initialize(m);
        float[] arr = new float[LayerManager.Instance.ChunkResolution * LayerManager.Instance.ChunkResolution * LayerManager.Instance.ChunkResolution];
        LayerManager.Instance.AddNewLayer();
        foreach (var chunk in LayerManager.Instance.ActiveLayer.chunks)
        {
            if (bounds.Intersects(chunk.ColliderBounds))
            {
                chunk.voxels.Initialized = true;
                LayerManager.Instance.activeChunks.Add(chunk);
                int kernel = meshToSdfShader.FindKernel("CalculateDistance");
                meshToSdfShader.SetBuffer(kernel, "inputTriangles", trianglesBuffer);
                meshToSdfShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
                meshToSdfShader.SetMatrix("modelMatrix", chunk.ModelMatrix);
                meshToSdfShader.SetInt("numPointsPerAxis", LayerManager.Instance.ChunkResolution);
                meshToSdfShader.SetInt("trianglesCount", trianglesCount);
                meshToSdfShader.SetFloat("chunkSize", LayerManager.Instance.Spacing);
                meshToSdfShader.SetFloat("sizeSquared", LayerManager.Instance.Size * LayerManager.Instance.Size);
                meshToSdfShader.SetFloat("maxValue", LayerManager.Instance.VoxelSpacing);
                meshToSdfShader.SetVector("boundsMin", new Vector4(bounds.min.x, bounds.min.y, bounds.min.z));
                meshToSdfShader.SetVector("boundsMax", new Vector4(bounds.max.x, bounds.max.y, bounds.max.z));

                meshToSdfShader.Dispatch(kernel, chunk.voxels.Resolution / 8, chunk.voxels.Resolution / 8, chunk.voxels.Resolution / 8);
            }
        }
        foreach (var chunk in LayerManager.Instance.activeChunks) chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        trianglesBuffer.Release();
    }
}