﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SnapshotController : MonoBehaviour, IDisposable
{
    public ComputeBuffer Snapshot { get => sdf; }
    public ComputeBuffer Colors { get => colors; }
    public GameObject ToFollow;
    public ComputeShader SnapshotShader;
    public int resolution;
    public int volume;
    public float size;
    public float spacing;
    ComputeBuffer sdf;
    ComputeBuffer colors;
    ComputeBuffer overlapCounter;
    public Material pointMaterial;
    Vector3Int gridPos;

    public void SetPosition(Vector3 pos)
    {
        transform.rotation = LayerManager.Instance.ActiveLayer.transform.rotation; 
        gridPos = LayerManager.Instance.SnapToGridPosition(pos);
        transform.position = (Vector3)gridPos * spacing;
    }

    public void SetPositionReal(Vector3 pos)
    {
        transform.rotation = LayerManager.Instance.ActiveLayer.transform.rotation;
        transform.position = LayerManager.Instance.SnapToGridPositionReal(pos);
        transform.localScale = LayerManager.Instance.ActiveLayer.transform.localScale;
    }

    public void SetCenter(Vector3 pos)
    {
        gridPos = LayerManager.Instance.SnapToGridPosition(pos);
        transform.position = (Vector3)gridPos * spacing - Vector3.one * size * 0.5f - Vector3.one * spacing * 0.5f;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(transform.position + Vector3.one * size * 0.5f, Vector3.one * size);
    //}

    void Start()
    {
        var manager = LayerManager.Instance;
        resolution = manager.ChunkResolution;
        size = manager.Size / manager.Resolution;
        spacing = size / (resolution - 1);
        volume = resolution * resolution * resolution;
        overlapCounter = new ComputeBuffer(volume, sizeof(uint));
        sdf = new ComputeBuffer(volume, sizeof(float));
        colors = new ComputeBuffer(volume, sizeof(float) * 3);
        transform.GetChild(0).transform.localScale = Vector3.one * size;
        //transform.GetChild(0).transform.localPosition = Vector3.one * size * 0.5f;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            SetPositionReal(ToFollow.transform.position);// - Vector3.one * size * 0.5f);
            //Debug.Log("Snapshot repositioned");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Snapshot taken");
            TakeSnapshot();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("Snapshot applied");
            ApplySnapshot();
        }
        //DisplayVoxels();
    }

    public void TakeSnapshot()
    {
        var overlappedColliders = Physics.OverlapBox(
            transform.position,
            Vector3.one * size * 0.5f,
            LayerManager.Instance.ActiveLayer.transform.rotation,
            1 << 9);
        var takeSnapshot = SnapshotShader.FindKernel("TakeSnapshot");
        var normalize = SnapshotShader.FindKernel("Normalize");
        var clearData = SnapshotShader.FindKernel("ClearData");
        SnapshotShader.SetBuffer(takeSnapshot, "snapshot", sdf);
        SnapshotShader.SetBuffer(takeSnapshot, "colors", colors);
        SnapshotShader.SetBuffer(takeSnapshot, "overlaps", overlapCounter);

        SnapshotShader.SetBuffer(normalize, "snapshot", sdf);
        SnapshotShader.SetBuffer(normalize, "colors", colors);
        SnapshotShader.SetBuffer(normalize, "overlaps", overlapCounter);

        SnapshotShader.SetBuffer(clearData, "snapshot", sdf);
        SnapshotShader.SetBuffer(clearData, "colors", colors);
        SnapshotShader.SetBuffer(clearData, "overlaps", overlapCounter);

        SnapshotShader.Dispatch(clearData, volume / 512, 1, 1);

        foreach (Collider collider in overlappedColliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();
            //Vector3 foo = LayerManager.Instance.ActiveLayer.transform.worldToLocalMatrix.MultiplyPoint(transform.position);
            Vector3 foo = transform.position;
            Vector3 snapLocalPos = chunk.transform.InverseTransformPoint(foo);
            snapLocalPos -= size * Vector3.one * 0.5f;
            //Vector3 snapLocalPos = chunk.transform.worldToLocalMatrix.MultiplyPoint(foo);
            SnapshotShader.SetBuffer(takeSnapshot, "sdf", chunk.voxels.VoxelBuffer);
            SnapshotShader.SetBuffer(takeSnapshot, "chunkColors", chunk.voxels.ColorBuffer);
            SnapshotShader.SetVector("gridDisplacement", snapLocalPos / spacing);
            //Debug.Log($"{snapLocalPos.x} {spacing} {(snapLocalPos/spacing).x}");
            SnapshotShader.SetInt("resolution", resolution);
            SnapshotShader.Dispatch(takeSnapshot, resolution / 8, resolution / 8, resolution / 8);
        }
        SnapshotShader.Dispatch(normalize, volume/512, 1, 1);
    }

    public void ApplySnapshot()
    {
        var overlappedColliders = Physics.OverlapBox(
            transform.position,
            Vector3.one * size * 0.5f,
            LayerManager.Instance.ActiveLayer.transform.rotation,
            1 << 9);
        var kernel = SnapshotShader.FindKernel("ApplySnapshot");
        SnapshotShader.SetBuffer(kernel, "snapshot", sdf);
        SnapshotShader.SetBuffer(kernel, "colors", colors);

        foreach (Collider collider in overlappedColliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();

            //Vector3 foo = LayerManager.Instance.ActiveLayer.transform.worldToLocalMatrix.MultiplyPoint(transform.position);
            Vector3 foo = transform.position;
            Vector3 snapLocalPos = chunk.transform.InverseTransformPoint(foo);
            snapLocalPos -= size * Vector3.one * 0.5f;

            //Vector3 snapLocalPos = chunk.transform.worldToLocalMatrix.MultiplyPoint(foo);
            SnapshotShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            SnapshotShader.SetBuffer(kernel, "chunkColors", chunk.voxels.ColorBuffer);
            SnapshotShader.SetVector("gridDisplacement", snapLocalPos / spacing);
            SnapshotShader.SetInt("resolution", resolution);
            SnapshotShader.Dispatch(kernel, resolution / 8, resolution / 8, resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        }
    }

    private void DisplayVoxels()
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", sdf);
        materialBlock.SetVector("offset", transform.position);
        materialBlock.SetFloat("spacing", spacing);
        materialBlock.SetInt("res", resolution);
        Graphics.DrawProcedural(
            pointMaterial,
            new Bounds(Vector3.zero, new Vector3(100, 100, 100)), //what exactly should go here?
            MeshTopology.Points,
            1,
            volume,
            null,
            materialBlock
            );
    }

    public void Dispose()
    {
        if (sdf != null) sdf.Release();
        if (colors != null) colors.Release();
        if (overlapCounter != null) overlapCounter.Release();
    }

    ~SnapshotController()
    {
        Dispose();
    }
}