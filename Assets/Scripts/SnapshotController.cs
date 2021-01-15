// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="SnapshotController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Class SnapshotController.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="System.IDisposable" />
public class SnapshotController : MonoBehaviour, IDisposable
{
    /// <summary>
    /// Gets the snapshot SDF.
    /// </summary>
    /// <value>The snapshot SDF.</value>
    public ComputeBuffer SnapshotSdf { get; private set; }
    /// <summary>
    /// Gets the snapshot colors.
    /// </summary>
    /// <value>The snapshot colors.</value>
    public ComputeBuffer SnapshotColors { get; private set; }
    ComputeBuffer OverlapCounter { get; set; }


    /// <summary>
    /// The snapshot shader
    /// </summary>
    public ComputeShader SnapshotShader;
    int takeSnapshotKernel;
    int applySnapshotKernel;
    int normalizeKernel;
    int clearKernel;

    /// <summary>
    /// Gets the spacing.
    /// </summary>
    /// <value>The spacing.</value>
    public float Spacing { get; private set; }
    /// <summary>
    /// Gets the resolution.
    /// </summary>
    /// <value>The resolution.</value>
    public int Resolution { get; private set; }
    /// <summary>
    /// Gets the volume.
    /// </summary>
    /// <value>The volume.</value>
    public int Volume { get; private set; }
    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public float Size { get; private set; }

    /// <summary>
    /// The debug mode
    /// </summary>
    [Header("Debug options")]
    public bool debugMode = false;
    /// <summary>
    /// Converts to follow.
    /// </summary>
    public GameObject ToFollow;
    /// <summary>
    /// The point material
    /// </summary>
    public Material pointMaterial;

    /// <summary>
    /// Sets the position real.
    /// </summary>
    /// <param name="pos">The position.</param>
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

    /// <summary>
    /// Takes the snapshot.
    /// </summary>
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

    /// <summary>
    /// Applies the snapshot.
    /// </summary>
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

    /// <summary>
    /// Disposes this instance.
    /// </summary>
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
