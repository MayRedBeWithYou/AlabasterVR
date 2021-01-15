using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour, IDisposable
{
    public int resolution;

    public float size;

    public Vector3Int coord = Vector3Int.zero;

    public Vector3 center
    {
        get
        {
            return transform.position + Vector3.one * size / 2;
        }
    }

    public Vector3 offset => transform.position;
    public Matrix4x4 ModelMatrix => transform.localToWorldMatrix;
    public Matrix4x4 InverseModelMatrix => transform.worldToLocalMatrix;
    
    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;
    private BoxCollider _boxCollider;
    public Bounds ColliderBounds { get {return _boxCollider.bounds;}  }
    public GPUVoxelData voxels;

    public GPUMesh gpuMesh;

    public bool DisplayVoxels;

    [HideInInspector]
    public Mesh mesh;

    public void Awake()
    {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();
        _boxCollider = GetComponent<BoxCollider>();

        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    public void Init()
    {
        voxels = new GPUVoxelData(resolution, size);
        gpuMesh = new GPUMesh(voxels.Volume * 5);
    }

    void Update()
    {
        if (DisplayVoxels) voxels.DrawVoxelData(offset);
        if (voxels.Initialized) gpuMesh.DrawMesh(ModelMatrix, InverseModelMatrix);
    }

    public void GenerateCollider()
    {
        _collider.enabled = false;
        _collider.enabled = true;
    }

    public void ToggleColliders(bool value)
    {
        _collider.enabled = value;
        _boxCollider.enabled = value;
    }

    void OnDestroy()
    {
        gpuMesh.Dispose();
        voxels.Dispose();
    }

    public void Dispose()
    {
        gpuMesh.Dispose();
        voxels.Dispose();
    }
}