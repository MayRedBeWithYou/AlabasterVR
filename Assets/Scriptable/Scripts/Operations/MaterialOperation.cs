using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOperation : ChunkEditOperation
{
    public MaterialOperation(Dictionary<Chunk, float[]> beforeSdf, Dictionary<Chunk, float[]> beforeCol, Dictionary<Chunk, float[]> afterSdf, Dictionary<Chunk, float[]> afterCol)
        : base(beforeSdf, beforeCol, afterSdf, afterCol)
    {
    }
}
