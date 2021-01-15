// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="CubeMaterialCursor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class CubeMaterialCursor.
/// Implements the <see cref="MaterialCursor" />
/// </summary>
/// <seealso cref="MaterialCursor" />
public class CubeMaterialCursor : MaterialCursor
{
    private CubeCursor cursor;

    private int addMaterialKernel;
    private int removeMaterialKernel;

    /// <summary>
    /// The shader
    /// </summary>
    public ComputeShader shader;

    /// <summary>
    /// Gets the add material kernel.
    /// </summary>
    /// <value>The add material kernel.</value>
    public override int AddMaterialKernel { get => addMaterialKernel; }
    /// <summary>
    /// Gets the remove material kernel.
    /// </summary>
    /// <value>The remove material kernel.</value>
    public override int RemoveMaterialKernel { get => removeMaterialKernel; }

    /// <summary>
    /// Prepares the shader.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>ComputeShader.</returns>
    public override ComputeShader PrepareShader(Color color)
    {
        shader.SetFloat("edge", cursor.Size);
        shader.SetVector("color", new Vector3(color.r, color.g, color.b));
        return shader;
    }

    /// <summary>
    /// Updates the active chunks.
    /// </summary>
    public override void UpdateActiveChunks()
    {
        cursor.UpdateActiveChunks();
    }

    protected void Awake()
    {
        cursor = GetComponent<CubeCursor>();
        InitializeShadersConstUniforms();
    }

    private void InitializeShadersConstUniforms()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;

        shader.SetFloat("chunkSize", activeLayer.Spacing);
        shader.SetInt("resolution", activeLayer.ChunkResolution);
        shader.SetFloat("voxelSpacing", LayerManager.Instance.VoxelSpacing);
        shader.SetFloat("edge", cursor.Size);
    }

    void Start()
    {
        addMaterialKernel = shader.FindKernel("AddMaterial");
        removeMaterialKernel = shader.FindKernel("RemoveMaterial");
    }

    /// <summary>
    /// Enables this instance.
    /// </summary>
    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables this instance.
    /// </summary>
    public override void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the color.
    /// </summary>
    /// <param name="color">The color.</param>
    public override void SetColor(Color color)
    {
        cursor.Color = color;
    }

    /// <summary>
    /// Increases the size.
    /// </summary>
    public override void IncreaseSize()
    {
        cursor.IncreaseSize();
    }

    /// <summary>
    /// Decreases the size.
    /// </summary>
    public override void DecreaseSize()
    {
        cursor.DecreaseSize();
    }

    /// <summary>
    /// Sets the maximum size.
    /// </summary>
    /// <param name="value">The value.</param>
    public override void SetMaximumSize(float value)
    {
        cursor.MaximalSize = value;
    }

    /// <summary>
    /// Sets the minimum size.
    /// </summary>
    /// <param name="value">The value.</param>
    public override void SetMinimumSize(float value)
    {
        cursor.MinimalSize = value;
    }

    /// <summary>
    /// Sets the size to default.
    /// </summary>
    public override void SetSizeToDefault()
    {
        cursor.SetSizeToDefault();
    }
}
