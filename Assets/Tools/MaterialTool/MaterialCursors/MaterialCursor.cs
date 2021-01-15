// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="MaterialCursor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class MaterialCursor.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public abstract class MaterialCursor : MonoBehaviour
{
    /// <summary>
    /// Gets the add material kernel.
    /// </summary>
    /// <value>The add material kernel.</value>
    abstract public int AddMaterialKernel { get; }
    /// <summary>
    /// Gets the remove material kernel.
    /// </summary>
    /// <value>The remove material kernel.</value>
    abstract public int RemoveMaterialKernel { get; }
        
    /// <summary>
    /// Prepares the shader with relevant info.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>ComputeShader.</returns>
    public abstract ComputeShader PrepareShader(Color color);

    /// <summary>
    /// Enables this instance.
    /// </summary>
    public abstract void Enable();

    /// <summary>
    /// Disables this instance.
    /// </summary>
    public abstract void Disable();

    /// <summary>
    /// Sets the color.
    /// </summary>
    /// <param name="color">The color.</param>
    public abstract void SetColor(Color color);

    /// <summary>
    /// Updates active chunks.
    /// </summary>
    public abstract void UpdateActiveChunks();
    /// <summary>
    /// Increases the size.
    /// </summary>
    public abstract void IncreaseSize();
    /// <summary>
    /// Decreases the size.
    /// </summary>
    public abstract void DecreaseSize();

    /// <summary>
    /// Sets the maximum size.
    /// </summary>
    /// <param name="value">The value.</param>
    public abstract void SetMaximumSize(float value);

    /// <summary>
    /// Sets the minimum size.
    /// </summary>
    /// <param name="value">The value.</param>
    public abstract void SetMinimumSize(float value);

    /// <summary>
    /// Sets the size to default.
    /// </summary>
    public abstract void SetSizeToDefault();
}
