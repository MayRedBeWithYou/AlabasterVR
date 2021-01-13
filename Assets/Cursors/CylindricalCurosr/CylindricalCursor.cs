using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylindricalCursor : BaseCursor
{
    private float _radius;
    public override float Size
    {
        get => _radius;
        protected set
        {
            _radius = value;
            transform.localScale = new Vector3((_radius * 2), transform.localScale.y, (_radius * 2));
        }
    }

    public override void UpdateActiveChunks()
    {
        var collidedChunks = Physics.OverlapSphere(transform.position, Size + 0.1f, 1 << 9);
        LayerManager.Instance.activeChunks.Clear();
        foreach (Collider col in collidedChunks)
        {
            LayerManager.Instance.activeChunks.Add(col.GetComponent<Chunk>());
        }
    }

    protected override void Awake()
    {
        base.Awake();
       // transform.localScale = new Vector3(1,LayerManager.Instance.VoxelSpacing,1);
    }
}
