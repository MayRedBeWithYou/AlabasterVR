using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPUVoxelData : IDisposable
{
    private ComputeBuffer _voxelBuffer;
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
    public int Resolution { get; private set; }
    public int Volume { get; private set; }
    public float Size { get; private set; }
    private static Material pointMaterial;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        pointMaterial = Resources.Load<Material>("Materials/PointMaterial");
    }

    public bool Initialized = false;

    public GPUVoxelData(int resolution, float size)
    {
        Size = size;
        Resolution = resolution;
        Volume = Resolution * Resolution * Resolution;
    }

    public void InitBuffer()
    {
        _voxelBuffer = new ComputeBuffer(Volume, sizeof(float));
        ResetVoxelsValues();
        Initialized = true;
    }

    private void ResetVoxelsValues()
    {
        VoxelBuffer.SetData(Enumerable.Repeat(1f, Volume).ToArray());
    }

    public void Dispose()
    {
        if (_voxelBuffer != null) VoxelBuffer.Dispose();
    }

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
    public void InitializeFromArray(float[] values)
    {
        VoxelBuffer.SetData(values);
    }
}
