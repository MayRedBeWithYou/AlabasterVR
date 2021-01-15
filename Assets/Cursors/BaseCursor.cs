// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="BaseCursor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class BaseCursor.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public abstract class BaseCursor : MonoBehaviour
{
    private Material _material;
    protected Transform childMesh;
    [SerializeField]
    private float alpha = 0.1f;
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
        get => _material.color;
        set
        {
            value.a = alpha;
            _material.color = value;
        }
    }
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public abstract float Size { get; protected set; }
    private float _maximalSize;
    private float _minimalSize;

    /// <summary>
    /// Gets or sets the size of the maximal.
    /// </summary>
    /// <value>The size of the maximal.</value>
    public float MaximalSize
    {
        get => _maximalSize;
        set
        {
            if (Size > value) Size = value;
            _maximalSize = value;
        }
    }
    /// <summary>
    /// Gets or sets the size of the minimal.
    /// </summary>
    /// <value>The size of the minimal.</value>
    public float MinimalSize
    {
        get => _minimalSize;
        set
        {
            if (Size < value) Size = value;
            _minimalSize = value;
        }
    }
    /// <summary>
    /// Sets the size to default.
    /// </summary>
    public void SetSizeToDefault()
    {
        Size = (MaximalSize - MinimalSize) * 0.3f;
    }
    /// <summary>
    /// The size change speed
    /// </summary>
    public float SizeChangeSpeed = 0.001f;

    /// <summary>
    /// Decreases the size.
    /// </summary>
    public void DecreaseSize()
    {
        Size -= SizeChangeSpeed;
        if (Size < MinimalSize) Size = MinimalSize;
    }

    /// <summary>
    /// Increases the size.
    /// </summary>
    public void IncreaseSize()
    {
        Size += SizeChangeSpeed;
        if (Size > MaximalSize) Size = MaximalSize;
    }

    /// <summary>
    /// Updates the active chunks.
    /// </summary>
    public abstract void UpdateActiveChunks();
    protected virtual void Awake()
    {
        _material = GetComponentInChildren<MeshRenderer>().material;
        childMesh = transform.GetChild(0);
    }
}
