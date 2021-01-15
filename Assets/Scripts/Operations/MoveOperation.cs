// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-10-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-10-2021
// ***********************************************************************
// <copyright file="MoveOperation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class MoveOperation.
/// Implements the <see cref="ChunkEditOperation" />
/// </summary>
/// <seealso cref="ChunkEditOperation" />
public class MoveOperation : ChunkEditOperation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoveOperation" /> class.
    /// </summary>
    /// <param name="beforeSdf">The before SDF.</param>
    /// <param name="beforeCol">The before col.</param>
    /// <param name="afterSdf">The after SDF.</param>
    /// <param name="afterCol">The after col.</param>
    public MoveOperation(Dictionary<Chunk, float[]> beforeSdf, Dictionary<Chunk, float[]> beforeCol, Dictionary<Chunk, float[]> afterSdf, Dictionary<Chunk, float[]> afterCol)
        : base(beforeSdf, beforeCol, afterSdf, afterCol)
    {
    }
}
