using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSDF : MonoBehaviour
{
    public float radius;

    public float scale;

    private MeshRenderer _renderer;

    private MeshCollider _meshCollider;

    private BoxCollider _boxCollider;

    public void Awake()
    {
        radius = transform.localScale.x/2;
        _renderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _boxCollider = GetComponent<BoxCollider>();
        GenerateCollider();
    }

    public void GenerateCollider()
    {
        _meshCollider.enabled = false;
        _meshCollider.enabled = true;
    }

    public void UpdateActiveChunks()
    {
        var collidedChunks = Physics.OverlapSphere(transform.position, radius+0.1f, 1 << 9);
        if (collidedChunks != null)
        {
            LayerManager.Instance.activeChunks.Clear();
            foreach(Collider col in collidedChunks)
            {
                LayerManager.Instance.activeChunks.Add(col.GetComponent<Chunk>());
            }
        }
    }

    public void SetMaterial(Material material)
    {
        _renderer.material = material;
    }

    public void ToggleRenderer(bool value)
    {
        _renderer.enabled = value;
    }

    public void IncreaseRadius()
    {
        radius += scale;
        transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
    }
    public void DecreaseRadius()
    {
        if (radius > scale) radius -= scale;
        transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
    }

    public float IsInside(Vector3 point)
    {
        Vector3 pos = transform.position;
        float value = (point.x - pos.x) * (point.x - pos.x) + (point.y - pos.y) * (point.y - pos.y) + (point.z - pos.z) * (point.z - pos.z) - radius * radius;
        return Mathf.Clamp(value*Mathf.Abs(value), -1f, 1f);
    }
}
