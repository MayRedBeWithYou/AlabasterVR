// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="CylindricalCursor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class CylindricalCursor.
/// Implements the <see cref="BaseCursor" />
/// </summary>
/// <seealso cref="BaseCursor" />
public class CylindricalCursor : BaseCursor
{
    private float _radius;
    private float _height;
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
            childMesh.localScale = new Vector3((_radius * 2), childMesh.localScale.y, (_radius * 2));
        }
    }

    /// <summary>
    /// Updates the active chunks.
    /// </summary>
    public override void UpdateActiveChunks()
    {
        var collidedChunks = Physics.OverlapBox(transform.position, new Vector3(_radius*1.1f, _height * 1.1f, _radius * 1.1f), transform.rotation, 1 << 9);
        LayerManager.Instance.activeChunks.Clear();
        foreach (Collider col in collidedChunks)
        {
            LayerManager.Instance.activeChunks.Add(col.GetComponent<Chunk>());
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _height = LayerManager.Instance.VoxelSpacing * 4;
        childMesh.localScale = new Vector3(Size * 2, _height, Size * 2);
    }
}
