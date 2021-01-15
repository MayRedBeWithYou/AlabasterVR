// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="Chunk.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Chunk.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="System.IDisposable" />
public class Chunk : MonoBehaviour, IDisposable
{
    /// <summary>
    /// The resolution
    /// </summary>
    public int resolution;

    /// <summary>
    /// The size
    /// </summary>
    public float size;

    /// <summary>
    /// The coord
    /// </summary>
    public Vector3Int coord = Vector3Int.zero;

    /// <summary>
    /// Gets the center.
    /// </summary>
    /// <value>The center.</value>
    public Vector3 center
    {
        get
        {
            return transform.position + Vector3.one * size / 2;
        }
    }

    /// <summary>
    /// Gets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public Vector3 offset => transform.position;
    /// <summary>
    /// Gets the model matrix.
    /// </summary>
    /// <value>The model matrix.</value>
    public Matrix4x4 ModelMatrix => transform.localToWorldMatrix;
    /// <summary>
    /// Gets the inverse model matrix.
    /// </summary>
    /// <value>The inverse model matrix.</value>
    public Matrix4x4 InverseModelMatrix => transform.worldToLocalMatrix;
    
    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;
    private BoxCollider _boxCollider;
    /// <summary>
    /// Gets the collider bounds.
    /// </summary>
    /// <value>The collider bounds.</value>
    public Bounds ColliderBounds { get {return _boxCollider.bounds;}  }
    /// <summary>
    /// The voxels
    /// </summary>
    public GPUVoxelData voxels;

    /// <summary>
    /// The gpu mesh
    /// </summary>
    public GPUMesh gpuMesh;

    /// <summary>
    /// The display voxels
    /// </summary>
    public bool DisplayVoxels;

    /// <summary>
    /// The mesh
    /// </summary>
    [HideInInspector]
    public Mesh mesh;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    public void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();
        _boxCollider = GetComponent<BoxCollider>();

        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Init()
    {
        voxels = new GPUVoxelData(resolution, size);
        gpuMesh = new GPUMesh(voxels.Volume * 5);
    }

    void Update()
    {
        if (DisplayVoxels) voxels.DrawVoxelData(offset);
        if (voxels.Initialized) gpuMesh.DrawMesh(ModelMatrix, InverseModelMatrix);
    }

    /// <summary>
    /// Generates the collider.
    /// </summary>
    public void GenerateCollider()
    {
        _collider.enabled = false;
        _collider.enabled = true;
    }

    /// <summary>
    /// Toggles the colliders.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public void ToggleColliders(bool value)
    {
        _collider.enabled = value;
        _boxCollider.enabled = value;
    }

    void OnDestroy()
    {
        gpuMesh.Dispose();
        voxels.Dispose();
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        gpuMesh.Dispose();
        voxels.Dispose();
    }
}