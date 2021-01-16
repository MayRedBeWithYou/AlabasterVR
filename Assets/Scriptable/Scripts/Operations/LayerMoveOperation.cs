using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TransformObject
{
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;

    public TransformObject(Transform transform)
    {
        position = transform.position;
        scale = transform.localScale;
        rotation = transform.rotation;
    }

    public TransformObject(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        this.position = position;
        this.scale = scale;
        this.rotation = rotation;
    }
}
public class LayerMoveOperation : IOperation
{
    Layer layer;
    TransformObject before;
    TransformObject after;

    public LayerMoveOperation(Layer layer, TransformObject before, TransformObject after)
    {
        this.layer = layer;
        this.before = before;
        this.after = after;
    }

    public void Apply()
    {
        layer.transform.position = after.position;
        layer.transform.localScale = after.scale;
        layer.transform.rotation = after.rotation;
    }

    public void Revert()
    {
        layer.transform.position = before.position;
        layer.transform.localScale = before.scale;
        layer.transform.rotation = before.rotation;
    }
}
