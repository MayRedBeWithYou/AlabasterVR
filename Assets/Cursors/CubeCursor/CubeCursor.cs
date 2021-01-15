// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="CubeCursor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class CubeCursor.
/// Implements the <see cref="BaseCursor" />
/// </summary>
/// <seealso cref="BaseCursor" />
public class CubeCursor : BaseCursor
{
    private float _edge;
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public override float Size
    {
        get => _edge;
        protected set
        {
            _edge = value;
            childMesh.localScale = Vector3.one * (_edge * 2);
        }
    }

    /// <summary>
    /// Updates the active chunks.
    /// </summary>
    public override void UpdateActiveChunks()
    {
        var collidedChunks = Physics.OverlapBox(transform.position, Vector3.one * (Size + 0.1f), transform.rotation, 1 << 9);
        LayerManager.Instance.activeChunks.Clear();
        foreach (Collider col in collidedChunks)
        {
            LayerManager.Instance.activeChunks.Add(col.GetComponent<Chunk>());
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
