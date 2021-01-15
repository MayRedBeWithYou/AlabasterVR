// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-07-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-07-2021
// ***********************************************************************
// <copyright file="LayerMoveOperation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class TransformObject.
/// </summary>
public class TransformObject
{
    /// <summary>
    /// The position
    /// </summary>
    public Vector3 position;
    /// <summary>
    /// The scale
    /// </summary>
    public Vector3 scale;
    /// <summary>
    /// The rotation
    /// </summary>
    public Quaternion rotation;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransformObject" /> class.
    /// </summary>
    /// <param name="transform">The transform.</param>
    public TransformObject(Transform transform)
    {
        position = transform.position;
        scale = transform.localScale;
        rotation = transform.rotation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransformObject" /> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="scale">The scale.</param>
    /// <param name="rotation">The rotation.</param>
    public TransformObject(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        this.position = position;
        this.scale = scale;
        this.rotation = rotation;
    }
}
/// <summary>
/// Class LayerMoveOperation.
/// Implements the <see cref="IOperation" />
/// </summary>
/// <seealso cref="IOperation" />
public class LayerMoveOperation : IOperation
{
    Layer layer;
    TransformObject before;
    TransformObject after;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayerMoveOperation" /> class.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <param name="before">The before.</param>
    /// <param name="after">The after.</param>
    public LayerMoveOperation(Layer layer, TransformObject before, TransformObject after)
    {
        this.layer = layer;
        this.before = before;
        this.after = after;
    }

    /// <summary>
    /// Applies this instance.
    /// </summary>
    public void Apply()
    {
        layer.transform.position = after.position;
        layer.transform.localScale = after.scale;
        layer.transform.rotation = after.rotation;
    }

    /// <summary>
    /// Reverts this instance.
    /// </summary>
    public void Revert()
    {
        layer.transform.position = before.position;
        layer.transform.localScale = before.scale;
        layer.transform.rotation = before.rotation;
    }
}
