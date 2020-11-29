using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPUVoxelData : IDisposable
{
    public ComputeBuffer VoxelBuffer { get; private set; }
    public int Resolution { get; private set; }
    public int Volume { get; private set; }
    public float Size { get; private set; }

    public GPUVoxelData(int resolution, float size)
    {
        Size = size;
        Resolution = resolution;
        Volume = Resolution * Resolution * Resolution;
        VoxelBuffer = new ComputeBuffer(Volume, sizeof(float));
        ResetVoxelsValues();
    }

    private void ResetVoxelsValues()
    {
        VoxelBuffer.SetData(Enumerable.Repeat(1f, Volume).ToArray());
    }

    public void Dispose()
    {
        VoxelBuffer.Dispose();
    }
}
