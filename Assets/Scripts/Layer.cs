// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="Layer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Layer.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="IMovable" />
/// Implements the <see cref="IResizable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="IMovable" />
/// <seealso cref="IResizable" />
public class Layer : MonoBehaviour, IMovable, IResizable
{
    /// <summary>
    /// The resolution
    /// </summary>
    [Header("Parameters")]

    public int Resolution;

    /// <summary>
    /// The size
    /// </summary>
    public float Size;

    /// <summary>
    /// The chunk resolution
    /// </summary>
    public int ChunkResolution;
    /// <summary>
    /// Gets the spacing.
    /// </summary>
    /// <value>The spacing.</value>
    public float Spacing => Size / Resolution;

    /// <summary>
    /// The chunks
    /// </summary>
    public Chunk[,,] chunks;
    private float metallic;
    private float smoothness;
    private RenderType renderType;

    /// <summary>
    /// Gets or sets the type of the render.
    /// </summary>
    /// <value>The type of the render.</value>
    public RenderType RenderType
    {
        get => renderType;
        set
        {
            renderType = value;
            if (chunks != null)
                foreach (Chunk c in chunks)
                {
                    c.gpuMesh.renderType = value;
                    c.gpuMesh.UpdateVertexBuffer(c.voxels);
                }
        }
    }

    /// <summary>
    /// Gets or sets the metallic.
    /// </summary>
    /// <value>The metallic.</value>
    public float Metallic
    {
        get => metallic;
        set
        {
            metallic = value;
            if (chunks != null)
                foreach (Chunk c in chunks)
                {
                    c.gpuMesh.metallic = value;
                }
        }
    }

    /// <summary>
    /// Gets or sets the smoothness.
    /// </summary>
    /// <value>The smoothness.</value>
    public float Smoothness
    {
        get => smoothness;
        set
        {
            smoothness = value;
            if (chunks != null)
                foreach (Chunk c in chunks)
                {
                    c.gpuMesh.smoothness = value;
                }
        }
    }

    /// <summary>
    /// Generates the chunks.
    /// </summary>
    /// <param name="ChunkPrefab">The chunk prefab.</param>
    public void GenerateChunks(GameObject ChunkPrefab)
    {
        chunks = new Chunk[Resolution, Resolution, Resolution];

        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                for (int z = 0; z < Resolution; z++)
                {
                    GameObject go = Instantiate(ChunkPrefab, transform);
                    go.transform.localPosition = new Vector3(x, y, z) * (LayerManager.Instance.VoxelSpacing * (ChunkResolution - 3)); //chunkRes - 3, because chunks have to overlap each other if we want to compute gradient
                    go.name = $"Chunk ({x},{y},{z})";
                    Chunk chunk = go.GetComponent<Chunk>();
                    chunk.coord = new Vector3Int(x, y, z);
                    chunk.resolution = ChunkResolution;
                    chunk.size = Spacing;
                    BoxCollider col = go.GetComponent<BoxCollider>();
                    col.center = Vector3.one * Spacing / 2f;
                    col.size = Vector3.one * Spacing;
                    chunk.Init();
                    chunk.gpuMesh.renderType = renderType;
                    chunks[x, y, z] = chunk;

                }
            }
        }
    }

    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="pos">The position.</param>
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    /// <summary>
    /// Sets the rotation.
    /// </summary>
    /// <param name="rot">The rot.</param>
    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    /// <param name="scale">The scale.</param>
    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
