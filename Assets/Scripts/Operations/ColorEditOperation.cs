// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-10-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-10-2021
// ***********************************************************************
// <copyright file="ColorEditOperation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ColorEditOperation.
/// Implements the <see cref="IOperation" />
/// </summary>
/// <seealso cref="IOperation" />
public class ColorEditOperation : IOperation
{
    Dictionary<Chunk, float[]> beforeColor;
    Dictionary<Chunk, float[]> afterColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorEditOperation" /> class.
    /// </summary>
    /// <param name="beforeCol">The before col.</param>
    /// <param name="afterCol">The after col.</param>
    public ColorEditOperation(Dictionary<Chunk, float[]> beforeCol, Dictionary<Chunk, float[]> afterCol)
    {
        beforeColor = beforeCol;
        afterColor = afterCol;
    }

    /// <summary>
    /// Applies this instance.
    /// </summary>
    public void Apply()
    {
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
        foreach (KeyValuePair<Chunk, float[]> item in beforeColor)
        {
            item.Key.voxels.ColorBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }
}
