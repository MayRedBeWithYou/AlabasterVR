using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SnapshotController : MonoBehaviour, IDisposable
{
    public ComputeBuffer SnapshotSdf { get; private set; }
    public ComputeBuffer SnapshotColors { get; private set; }
    ComputeBuffer OverlapCounter { get; set; }

    
    public ComputeShader SnapshotShader;
    int takeSnapshotKernel;
    int applySnapshotKernel;
    int normalizeKernel;
    int clearKernel;

    public float Spacing { get; private set; }
    public int Resolution { get; private set; }
    public int Volume { get; private set; }
    public float Size { get; private set; }

    [Header("Debug options")]
    public bool debugMode = false;
    public GameObject ToFollow;
    public Material pointMaterial;

    public void SetPositionReal(Vector3 pos)
    {
        transform.rotation = LayerManager.Instance.ActiveLayer.transform.rotation;
        transform.position = LayerManager.Instance.SnapToGridPositionReal(pos);
        transform.localScale = LayerManager.Instance.ActiveLayer.transform.localScale;
    }

    void Start()
    {
        var manager = LayerManager.Instance;
        Resolution = manager.ChunkResolution;
        Size = manager.Size / manager.Resolution;
        Spacing = Size / (Resolution - 1);
        Volume = Resolution * Resolution * Resolution;
        OverlapCounter = new ComputeBuffer(Volume, sizeof(uint));
        SnapshotSdf = new ComputeBuffer(Volume, sizeof(float));
        SnapshotColors = new ComputeBuffer(Volume, sizeof(float) * 3);
        transform.GetChild(0).transform.localScale = Vector3.one * Size;
        InitializeShadersConstUniforms();
    }

    void Update()
    {
        if(debugMode)
        {
            if(Input.GetKeyDown(KeyCode.Alpha8))
            {
                SetPositionReal(ToFollow.transform.position);
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
            DisplayVoxels();
        }
    }

    private void InitializeShadersConstUniforms()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        takeSnapshotKernel = SnapshotShader.FindKernel("TakeSnapshot");
        normalizeKernel = SnapshotShader.FindKernel("Normalize");
        clearKernel = SnapshotShader.FindKernel("ClearData");
        applySnapshotKernel = SnapshotShader.FindKernel("ApplySnapshot");

        SnapshotShader.SetBuffer(takeSnapshotKernel, "snapshot", SnapshotSdf);
        SnapshotShader.SetBuffer(takeSnapshotKernel, "colors", SnapshotColors);
        SnapshotShader.SetBuffer(takeSnapshotKernel, "overlaps", OverlapCounter);
        SnapshotShader.SetBuffer(normalizeKernel, "snapshot", SnapshotSdf);
        SnapshotShader.SetBuffer(normalizeKernel, "colors", SnapshotColors);
        SnapshotShader.SetBuffer(normalizeKernel, "overlaps", OverlapCounter);
        SnapshotShader.SetBuffer(clearKernel, "snapshot", SnapshotSdf);
        SnapshotShader.SetBuffer(clearKernel, "colors", SnapshotColors);
        SnapshotShader.SetBuffer(clearKernel, "overlaps", OverlapCounter); 
        SnapshotShader.SetBuffer(applySnapshotKernel, "snapshot", SnapshotSdf);
        SnapshotShader.SetBuffer(applySnapshotKernel, "colors", SnapshotColors);
        SnapshotShader.SetFloat("voxelSpacing", Spacing);
        SnapshotShader.SetInt("resolution", Resolution);
    }

    public void TakeSnapshot()
    {
        var overlappedColliders = Physics.OverlapBox(
            transform.position,
            Vector3.one * Size * 0.5f,
            LayerManager.Instance.ActiveLayer.transform.rotation,
            1 << 9);
        
        SnapshotShader.Dispatch(clearKernel, Volume / 512, 1, 1);
        foreach (Collider collider in overlappedColliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();
            Vector3 snapLocalPos = chunk.transform.InverseTransformPoint(transform.position);
            snapLocalPos -= Size * Vector3.one * 0.5f;
            SnapshotShader.SetBuffer(takeSnapshotKernel, "sdf", chunk.voxels.VoxelBuffer);
            SnapshotShader.SetBuffer(takeSnapshotKernel, "chunkColors", chunk.voxels.ColorBuffer);
            SnapshotShader.SetVector("gridDisplacement", snapLocalPos / Spacing);
            SnapshotShader.Dispatch(takeSnapshotKernel, Resolution / 8, Resolution / 8, Resolution / 8);
        }
        SnapshotShader.Dispatch(normalizeKernel, Volume/512, 1, 1);
    }

    public void ApplySnapshot()
    {
        var overlappedColliders = Physics.OverlapBox(
            transform.position,
            Vector3.one * Size * 0.5f,
            LayerManager.Instance.ActiveLayer.transform.rotation,
            1 << 9);
        
        foreach (Collider collider in overlappedColliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();

            Vector3 snapLocalPos = chunk.transform.InverseTransformPoint(transform.position);
            snapLocalPos -= Size * Vector3.one * 0.5f;

            SnapshotShader.SetBuffer(applySnapshotKernel, "sdf", chunk.voxels.VoxelBuffer);
            SnapshotShader.SetBuffer(applySnapshotKernel, "chunkColors", chunk.voxels.ColorBuffer);
            SnapshotShader.SetVector("gridDisplacement", snapLocalPos / Spacing);
            SnapshotShader.Dispatch(applySnapshotKernel, Resolution / 8, Resolution / 8, Resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        }
    }

    private void DisplayVoxels()
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", SnapshotSdf);
        materialBlock.SetVector("offset", transform.position - Vector3.one * Size * 0.5f);
        materialBlock.SetFloat("spacing", Spacing);
        materialBlock.SetInt("res", Resolution);
        Graphics.DrawProcedural(
            pointMaterial,
            new Bounds(Vector3.zero, new Vector3(100, 100, 100)), //what exactly should go here?
            MeshTopology.Points,
            1,
            Volume,
            null,
            materialBlock
            );
    }

    public void Dispose()
    {
        if (SnapshotSdf != null) SnapshotSdf.Release();
        if (SnapshotColors != null) SnapshotColors.Release();
        if (OverlapCounter != null) OverlapCounter.Release();
    }

    ~SnapshotController()
    {
        Dispose();
    }
}
