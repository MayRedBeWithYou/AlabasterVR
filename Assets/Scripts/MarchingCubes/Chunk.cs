using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int size;

    public float boundSize;

    public Vector3Int coord = Vector3Int.zero;

    public Vector3 center
    {
        get
        {
            return (Vector3)coord * boundSize + Vector3.one * boundSize / 2;
        }
    }

    public Vector3 offset
    {
        get
        {
            return center - Vector3.one * boundSize / 2f;
        }
    }

    [HideInInspector]
    public Mesh mesh;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;

    [HideInInspector]
    public VoxelData[] voxelArray;


    public int GetVoxelIndex(int x, int y, int z)
    {
        return x + y * size + z * size * size;
    }    

    public void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();

        mesh = new Mesh();
        mesh.MarkDynamic();
        _filter.sharedMesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    public void GenerateCollider()
    {
        _collider.enabled = false;
        _collider.enabled = true;
    }
}

public struct VoxelData
{
    public Vector3 position;
    public float value;
}