// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-10-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="ChunkEditOperation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ChunkEditOperation.
/// Implements the <see cref="IOperation" />
/// </summary>
/// <seealso cref="IOperation" />
public class ChunkEditOperation : IOperation
{
    Dictionary<Chunk, float[]> beforeSDF;
    Dictionary<Chunk, float[]> afterSDF;

    Dictionary<Chunk, float[]> beforeColor;
    Dictionary<Chunk, float[]> afterColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChunkEditOperation" /> class.
    /// </summary>
    /// <param name="beforeSdf">The before SDF.</param>
    /// <param name="beforeCol">The before col.</param>
    /// <param name="afterSdf">The after SDF.</param>
    /// <param name="afterCol">The after col.</param>
    public ChunkEditOperation(Dictionary<Chunk, float[]> beforeSdf, Dictionary<Chunk, float[]> beforeCol, Dictionary<Chunk, float[]> afterSdf, Dictionary<Chunk, float[]> afterCol)
    {
        beforeSDF = beforeSdf;
        beforeColor = beforeCol;
        afterSDF = afterSdf;
        afterColor = afterCol;
    }

    /// <summary>
    /// Applies this instance.
    /// </summary>
    public void Apply()
    {
        foreach (KeyValuePair<Chunk, float[]> item in afterSDF)
        {
            item.Key.voxels.VoxelBuffer.SetData(item.Value);
        }
        foreach (KeyValuePair<Chunk, float[]> item in afterColor)
        {
            item.Key.voxels.ColorBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }

    /// <summary>
    /// Reverts this instance.
    /// </summary>
    public void Revert()
    {
        foreach (KeyValuePair<Chunk, float[]> item in beforeSDF)
        {
            item.Key.voxels.VoxelBuffer.SetData(item.Value);
        }
        foreach (KeyValuePair<Chunk, float[]> item in beforeColor)
        {
            item.Key.voxels.ColorBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }
}
