﻿using System;
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

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;
    private BoxCollider _boxCollider;

    public GPUVoxelData voxels;

    public GPUMesh gpuMesh;

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
        if (voxels.Initialized) gpuMesh.DrawMesh(offset);
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