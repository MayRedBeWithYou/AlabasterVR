using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCursor : BaseCursor
{
    private float _edge;
    public override float Size
    {
        get => _edge;
        protected set
        {
            _edge = value;
            childMesh.localScale = Vector3.one * (_edge * 2);
        }
    }

    public override void UpdateActiveChunks()
    {
        var collidedChunks = Physics.OverlapBox(transform.position, Vector3.one * (Size + 0.1f), transform.rotation, 1 << 9);
        LayerManager.Instance.activeChunks.Clear();
        foreach (Collider col in collidedChunks)
        {
            LayerManager.Instance.activeChunks.Add(col.GetComponent<Chunk>());
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
