using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MultiLayerMoveOperation : IOperation
{
    GameObject layerHelper;
    TransformObject before;
    TransformObject after;

    public MultiLayerMoveOperation(GameObject layerHelper, TransformObject before, TransformObject after)
    {
        this.layerHelper = layerHelper;
        this.before = before;
        this.after = after;
    }

    public void Apply()
    {
        foreach (Layer layer in LayerManager.Instance.layers) layer.transform.parent = layerHelper.transform;
        layerHelper.transform.position = after.position;
        layerHelper.transform.localScale = after.scale;
        layerHelper.transform.rotation = after.rotation;
        foreach (Layer layer in LayerManager.Instance.layers) layer.transform.parent = LayerManager.Instance.LayersHolder.transform;
    }

    public void Revert()
    {
        foreach (Layer layer in LayerManager.Instance.layers) layer.transform.parent = layerHelper.transform;
        layerHelper.transform.position = before.position;
        layerHelper.transform.localScale = before.scale;
        layerHelper.transform.rotation = before.rotation;
        foreach (Layer layer in LayerManager.Instance.layers) layer.transform.parent = LayerManager.Instance.LayersHolder.transform;
    }
}
