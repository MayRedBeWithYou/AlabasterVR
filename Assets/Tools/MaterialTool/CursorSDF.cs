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
        GenerateCollider();
        Vector3 center = _meshCollider.bounds.center;
        Vector3 size = _meshCollider.bounds.size * 0.5f;

        Layer activeLayer = LayerManager.Instance.activeLayer;

        float x1 = center.x - size.x;
        float x2 = center.x + size.x;
        float y1 = center.y - size.y;
        float y2 = center.y + size.y;
        float z1 = center.z - size.z;
        float z2 = center.z + size.z;

        int minX = (int)Mathf.Clamp(x1, 0, activeLayer.chunks.GetLength(0));
        int maxX = Mathf.Clamp(Mathf.CeilToInt(x2), 0, activeLayer.chunks.GetLength(0));
        int minY = (int)Mathf.Clamp(y1, 0, activeLayer.chunks.GetLength(0));
        int maxY = Mathf.Clamp(Mathf.CeilToInt(y2), 0, activeLayer.chunks.GetLength(0));
        int minZ = (int)Mathf.Clamp(z1, 0, activeLayer.chunks.GetLength(0));
        int maxZ = Mathf.Clamp(Mathf.CeilToInt(z2), 0, activeLayer.chunks.GetLength(0));

        LayerManager.Instance.activeChunks.Clear();
        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                for (int z = minZ; z < maxZ; z++)
                {
                    LayerManager.Instance.activeChunks.Add(activeLayer.chunks[x, y, z]);
                }
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
