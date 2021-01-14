using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylindricalCursor : BaseCursor
{
    private float _radius;
    private float _height;
    public override float Size
    {
        get => _radius;
        protected set
        {
            _radius = value;
            childMesh.localScale = new Vector3((_radius * 2), childMesh.localScale.y, (_radius * 2));
        }
    }

    public override void UpdateActiveChunks()
    {
        var collidedChunks = Physics.OverlapBox(transform.position, new Vector3(_radius*1.1f, _height * 1.1f, _radius * 1.1f), transform.rotation, 1 << 9);
        LayerManager.Instance.activeChunks.Clear();
        foreach (Collider col in collidedChunks)
        {
            LayerManager.Instance.activeChunks.Add(col.GetComponent<Chunk>());
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _height = LayerManager.Instance.VoxelSpacing * 4;
        childMesh.localScale = new Vector3(Size * 2, _height, Size * 2);
    }
}
