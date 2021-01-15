// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-11-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-11-2020
// ***********************************************************************
// <copyright file="MeshGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class MeshGenerator.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class MeshGenerator : MonoBehaviour
{
    private static MeshGenerator _instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static MeshGenerator Instance => _instance;

    /// <summary>
    /// The thread group size
    /// </summary>
    public int threadGroupSize = 8;

    /// <summary>
    /// The shader
    /// </summary>
    public ComputeShader shader;

    /// <summary>
    /// The display debug
    /// </summary>
    public bool displayDebug;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    /// <summary>
    /// Updates all active chunks.
    /// </summary>
    public void UpdateAllActiveChunks()
    {
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
            UpdateChunkMesh(chunk);
    }

    /// <summary>
    /// Updates the chunk mesh.
    /// </summary>
    /// <param name="chunk">The chunk.</param>
    public void UpdateChunkMesh(Chunk chunk)
    {
       // chunk.gpuMesh.DrawMesh(chunk.WorldMatrix);
    }
}
