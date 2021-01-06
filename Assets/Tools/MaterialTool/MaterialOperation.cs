using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOperation : IOperation
{
    Dictionary<Chunk, float[]> before;
    Dictionary<Chunk, float[]> after;

    public MaterialOperation(Dictionary<Chunk, float[]> beforeEdit, Dictionary<Chunk, float[]> afterEdit)
    {
        before = beforeEdit;
        after = afterEdit;
    }

    public void Apply()
    {
        foreach(KeyValuePair<Chunk, float[]> item in after)
        {
            item.Key.voxels.VoxelBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }

    public void Revert()
    {
        foreach (KeyValuePair<Chunk, float[]> item in before)
        {
            item.Key.voxels.VoxelBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }
}
