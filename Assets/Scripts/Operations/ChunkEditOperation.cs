using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkEditOperation : IOperation
{
    Dictionary<Chunk, float[]> beforeSDF;
    Dictionary<Chunk, float[]> afterSDF;

    Dictionary<Chunk, float[]> beforeColor;
    Dictionary<Chunk, float[]> afterColor;

    public ChunkEditOperation(Dictionary<Chunk, float[]> beforeSdf, Dictionary<Chunk, float[]> beforeCol, Dictionary<Chunk, float[]> afterSdf, Dictionary<Chunk, float[]> afterCol)
    {
        beforeSDF = beforeSdf;
        beforeColor = beforeCol;
        afterSDF = afterSdf;
        afterColor = afterCol;
    }

    public void Apply()
    {
        foreach (KeyValuePair<Chunk, float[]> item in afterSDF)
        {
            item.Key.voxels.VoxelBuffer.SetData(item.Value);
        }
        foreach (KeyValuePair<Chunk, float[]> item in afterColor)
        {
            item.Key.voxels.ColorBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }

    public void Revert()
    {
        foreach (KeyValuePair<Chunk, float[]> item in beforeSDF)
        {
            item.Key.voxels.VoxelBuffer.SetData(item.Value);
        }
        foreach (KeyValuePair<Chunk, float[]> item in beforeColor)
        {
            item.Key.voxels.ColorBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }
}
