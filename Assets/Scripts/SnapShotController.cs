using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SnapshotController : MonoBehaviour, IDisposable
{
    public ComputeBuffer Snapshot { get => data; }
    public GameObject ToFollow;
    public ComputeShader SnapshotShader;
    public int resolution;
    public int volume;
    public float size;
    public float spacing;
    ComputeBuffer data;
    ComputeBuffer overlapCounter;
    public Material pointMaterial;
    Vector3Int gridPos;

    public void SetPosition(Vector3 pos)
    {
        gridPos = LayerManager.Instance.SnapToGridPosition(pos);
        transform.position = (Vector3)gridPos * spacing;
    }

    public void SetCenter(Vector3 pos)
    {
        gridPos = LayerManager.Instance.SnapToGridPosition(pos);
        transform.position = (Vector3)gridPos * spacing - Vector3.one * size * 0.5f - Vector3.one * spacing * 0.5f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.one * size * 0.5f, Vector3.one * size);
    }

    void Start()
    {
        var manager = LayerManager.Instance;
        resolution = manager.ChunkResolution;
        size = manager.Size / manager.Resolution;
        spacing = size / (resolution - 1);
        volume = resolution * resolution * resolution;
        overlapCounter = new ComputeBuffer(volume, sizeof(uint));
        data = new ComputeBuffer(volume, sizeof(float));
    }

    void Update()
    {
        //SetPosition(ToFollow.transform.position - Vector3.one * size * 0.5f);
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            TakeSnapshot();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ApplySnapshot();
        }
        //DisplayVoxels();
    }

    public void TakeSnapshot()
    {
        var overlappedColliders = Physics.OverlapBox(
            transform.position + Vector3.one*size*0.5f,
            Vector3.one * size * 0.5f,
            Quaternion.identity,
            1 << 9);
        var kernel = SnapshotShader.FindKernel("TakeSnapshot");
        var normalize = SnapshotShader.FindKernel("Normalize");
        var clearData = SnapshotShader.FindKernel("ClearData");
        SnapshotShader.SetBuffer(kernel, "snapshot", data);
        SnapshotShader.SetBuffer(kernel, "overlaps", overlapCounter);

        SnapshotShader.SetBuffer(normalize, "snapshot", data);
        SnapshotShader.SetBuffer(normalize, "overlaps", overlapCounter);

        SnapshotShader.SetBuffer(clearData, "snapshot", data);
        SnapshotShader.SetBuffer(clearData, "overlaps", overlapCounter);

        SnapshotShader.Dispatch(clearData, volume / 512, 1, 1);

        foreach (Collider collider in overlappedColliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();

            Vector3 snapLocalPos = chunk.transform.worldToLocalMatrix.MultiplyPoint(transform.position);
            SnapshotShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            SnapshotShader.SetVector("gridDisplacement", snapLocalPos / spacing);
            SnapshotShader.SetInt("resolution", resolution);
            SnapshotShader.Dispatch(kernel, resolution / 8, resolution / 8, resolution / 8);
        }
        SnapshotShader.Dispatch(normalize, volume/512, 1, 1);
    }

    public void ApplySnapshot()
    {
        var overlappedColliders = Physics.OverlapBox(
            transform.position + Vector3.one * size * 0.5f,
            Vector3.one * size * 0.5f,
            LayerManager.Instance.ActiveLayer.transform.rotation,
            1 << 9);
        var kernel = SnapshotShader.FindKernel("ApplySnapshot");
        SnapshotShader.SetBuffer(kernel, "snapshot", data);

        foreach (Collider collider in overlappedColliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();

            Vector3 snapLocalPos = chunk.transform.worldToLocalMatrix.MultiplyPoint(transform.position);
            SnapshotShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            SnapshotShader.SetVector("gridDisplacement", snapLocalPos / spacing);
            SnapshotShader.SetInt("resolution", resolution);
            SnapshotShader.Dispatch(kernel, resolution / 8, resolution / 8, resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);

        }
    }

    private void DisplayVoxels()
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", data);
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
        if (data != null) data.Dispose();
        if (overlapCounter != null) overlapCounter.Dispose();
    }
}
