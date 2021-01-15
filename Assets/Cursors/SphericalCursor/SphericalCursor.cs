// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="SphericalCursor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class SphericalCursor.
/// Implements the <see cref="BaseCursor" />
/// </summary>
/// <seealso cref="BaseCursor" />
public class SphericalCursor : BaseCursor
{
    private float _radius;
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public override float Size
    {
        get => _radius;
        protected set
        {
            _radius = value;
            childMesh.localScale = Vector3.one * (_radius * 2);
        }
    }

    /// <summary>
    /// Updates the active chunks.
    /// </summary>
    public override void UpdateActiveChunks()
    {
        var collidedChunks = Physics.OverlapSphere(transform.position, Size + 0.1f, 1 << 9);
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
