// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="GPUVoxelData.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class GPUVoxelData.
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="System.IDisposable" />
public class GPUVoxelData : IDisposable
{
    private ComputeBuffer _voxelBuffer;
    private ComputeBuffer _colorBuffer;
    /// <summary>
    /// Gets the voxel buffer.
    /// </summary>
    /// <value>The voxel buffer.</value>
    public ComputeBuffer VoxelBuffer
    {
        get
        {
            if (_voxelBuffer is null)
            {
                InitBuffer();
            }
            return _voxelBuffer;
        }

        private set => _voxelBuffer = value;
    }

    /// <summary>
    /// Gets the color buffer.
    /// </summary>
    /// <value>The color buffer.</value>
    public ComputeBuffer ColorBuffer
    {
        get
        {
            if (_colorBuffer is null)
            {
                InitBuffer();
            }
            return _colorBuffer;
        }

        private set => _colorBuffer = value;
    }
    /// <summary>
    /// Gets the resolution.
    /// </summary>
    /// <value>The resolution.</value>
    public int Resolution { get; private set; }
    /// <summary>
    /// Gets the volume.
    /// </summary>
    /// <value>The volume.</value>
    public int Volume { get; private set; }
    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public float Size { get; private set; }
    private static Material pointMaterial;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        pointMaterial = Resources.Load<Material>("Materials/PointMaterial");
    }

    /// <summary>
    /// The initialized
    /// </summary>
    public bool Initialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="GPUVoxelData" /> class.
    /// </summary>
    /// <param name="resolution">The resolution.</param>
    /// <param name="size">The size.</param>
    public GPUVoxelData(int resolution, float size)
    {
        Size = size;
        Resolution = resolution;
        Volume = Resolution * Resolution * Resolution;
    }

    /// <summary>
    /// Initializes the buffer.
    /// </summary>
    public void InitBuffer()
    {
        _voxelBuffer = new ComputeBuffer(Volume, sizeof(float));
        _colorBuffer = new ComputeBuffer(Volume, sizeof(float) * 3);
        ResetVoxelsValues();
        Initialized = true;
    }

    private void ResetVoxelsValues()
    {
        VoxelBuffer.SetData(Enumerable.Repeat(LayerManager.Instance.VoxelSpacing, Volume).ToArray());
        ColorBuffer.SetData(Enumerable.Repeat(1.0f, Volume * 3).ToArray());
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        if (_voxelBuffer != null) VoxelBuffer.Dispose();
        if (_colorBuffer != null) ColorBuffer.Dispose();
    }

    /// <summary>
    /// Draws the voxel data.
    /// </summary>
    /// <param name="offset">The offset.</param>
    public void DrawVoxelData(Vector3 offset)
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", VoxelBuffer);
        materialBlock.SetVector("offset", offset);
        materialBlock.SetFloat("spacing", Size / (Resolution - 1));
        materialBlock.SetInt("res", Resolution);
        Graphics.DrawProcedural(
            pointMaterial,
            new Bounds(Vector3.zero, new Vector3(100, 100, 100)), //what exactly should go here?
            MeshTopology.Points,
            1,
            Volume,
            null,
            materialBlock
            );
    }
    /// <summary>
    /// Initializes from array.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="colors">The colors.</param>
    public void InitializeFromArray(float[] values, float[] colors = null)
    {
        if (_voxelBuffer == null)
        {
            _voxelBuffer = new ComputeBuffer(Volume, sizeof(float));
            _colorBuffer = new ComputeBuffer(Volume, sizeof(float) * 3);
        }
        VoxelBuffer.SetData(values);
        if (colors != null) ColorBuffer.SetData(colors);
        Initialized = true;
    }
}
