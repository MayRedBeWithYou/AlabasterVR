using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorEditOperation : IOperation
{
    Dictionary<Chunk, float[]> beforeColor;
    Dictionary<Chunk, float[]> afterColor;

    public ColorEditOperation(Dictionary<Chunk, float[]> beforeCol, Dictionary<Chunk, float[]> afterCol)
    {
        beforeColor = beforeCol;
        afterColor = afterCol;
    }

    public void Apply()
    {
        foreach (KeyValuePair<Chunk, float[]> item in afterColor)
        {
            item.Key.voxels.ColorBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }

    public void Revert()
    {
        foreach (KeyValuePair<Chunk, float[]> item in beforeColor)
        {
            item.Key.voxels.ColorBuffer.SetData(item.Value);
            item.Key.gpuMesh.UpdateVertexBuffer(item.Key.voxels);
        }
    }
}
