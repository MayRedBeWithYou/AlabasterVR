﻿// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="SmoothTool.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class SmoothTool.
/// Implements the <see cref="Tool" />
/// </summary>
/// <seealso cref="Tool" />
public class SmoothTool : Tool
{
    /// <summary>
    /// Struct SmoothData
    /// </summary>
    public struct SmoothData
    {
        /// <summary>
        /// From
        /// </summary>
        public Vector3Int from;
        /// <summary>
        /// The average
        /// </summary>
        public float avg;
        /// <summary>
        /// The average color
        /// </summary>
        public Vector3 avgColor;
    }

    /// <summary>
    /// The position button
    /// </summary>
    [Header("Handlers")]
    public ButtonHandler positionButton;

    /// <summary>
    /// Up button
    /// </summary>
    public ButtonHandler upButton;
    /// <summary>
    /// Down button
    /// </summary>
    public ButtonHandler downButton;

    /// <summary>
    /// The snapshot
    /// </summary>
    [Space(10)]

    public SnapshotController snapshot;

    /// <summary>
    /// The trigger
    /// </summary>
    public AxisHandler trigger;
    /// <summary>
    /// The smooth shader
    /// </summary>
    public ComputeShader SmoothShader;

    private ComputeBuffer workBuffer;
    private ComputeBuffer countBuffer;

    private int volume;

    private int populateWorkBufferKernel;
    private int applyWorkBufferKernel;

    /// <summary>
    /// The is working
    /// </summary>
    public bool isWorking = false;
    /// <summary>
    /// The before edit
    /// </summary>
    public Dictionary<Chunk, float[]> beforeEdit;
    /// <summary>
    /// The before color
    /// </summary>
    public Dictionary<Chunk, float[]> beforeColor;

    /// <summary>
    /// Enables this instance.
    /// </summary>
    public override void Enable()
    {
        if (!gameObject.activeSelf)
        {
            positionButton.OnButtonDown += PositionButton_OnButtonDown;
            positionButton.OnButtonUp += PositionButton_OnButtonUp;
            base.Enable();
        }
    }

    /// <summary>
    /// Disables this instance.
    /// </summary>
    public override void Disable()
    {
        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;
        base.Disable();
    }

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        snapshot = GameObject.Find("Snapshot").GetComponent<SnapshotController>();
        int res = LayerManager.Instance.ChunkResolution;
        volume = res * res * res;
        countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        workBuffer = new ComputeBuffer(volume, sizeof(float) * 4 + sizeof(uint) * 3, ComputeBufferType.Append);
        populateWorkBufferKernel = SmoothShader.FindKernel("PopulateWorkBuffer");
        applyWorkBufferKernel = SmoothShader.FindKernel("ApplySmooth");
    }

    private void Update()
    {
        if (upButton.IsPressed)
        {
            cursor.IncreaseSize();
        }
        if (downButton.IsPressed)
        {
            cursor.DecreaseSize();
        }
        cursor.UpdateActiveChunks();

        if (isWorking)
        {
            if (trigger.Value <= 0.2)
            {
                isWorking = false;
                Dictionary<Chunk, float[]> afterEdit = new Dictionary<Chunk, float[]>();
                Dictionary<Chunk, float[]> afterColor = new Dictionary<Chunk, float[]>();
                foreach (Chunk chunk in beforeEdit.Keys)
                {
                    float[] voxels = new float[chunk.voxels.Volume];
                    float[] colors = new float[chunk.voxels.Volume * 3];
                    chunk.voxels.VoxelBuffer.GetData(voxels);
                    chunk.voxels.ColorBuffer.GetData(colors);
                    afterEdit.Add(chunk, voxels);
                    afterColor.Add(chunk, colors);
                }

                SmoothOperation op = new SmoothOperation(beforeEdit, beforeColor, afterEdit, afterColor);
                OperationManager.Instance.PushOperation(op);
            }

            foreach (Chunk chunk in LayerManager.Instance.activeChunks)
            {
                if (!beforeEdit.ContainsKey(chunk))
                {
                    float[] voxels = new float[chunk.voxels.Volume];
                    float[] colors = new float[chunk.voxels.Volume * 3];
                    chunk.voxels.VoxelBuffer.GetData(voxels);
                    chunk.voxels.ColorBuffer.GetData(colors);
                    beforeEdit.Add(chunk, voxels);
                    beforeColor.Add(chunk, colors);
                }
            }
            PerformAction();
        }
        if (trigger.Value > 0.2)
        {
            if (!isWorking)
            {
                isWorking = true;
                beforeEdit = new Dictionary<Chunk, float[]>();
                beforeColor = new Dictionary<Chunk, float[]>();
            }
        }

    }

    private void PerformAction()
    {
        Smooth();
    }

    private void Smooth()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;

        snapshot.SetPositionReal(cursor.transform.position);
        snapshot.TakeSnapshot();

        workBuffer.SetCounterValue(0);
        var kernel = SmoothShader.FindKernel("PopulateWorkBuffer");
        SmoothShader.SetFloat("toolRadius", cursor.Size * 0.8f * (1f / snapshot.transform.localScale.x));
        SmoothShader.SetFloat("chunkSize", snapshot.Size);
        SmoothShader.SetVector("toolCenter", snapshot.transform.InverseTransformPoint(cursor.transform.position) + Vector3.one * snapshot.Size * 0.5f);

        SmoothShader.SetInt("resolution", snapshot.Resolution);
        SmoothShader.SetBuffer(populateWorkBufferKernel, "sdf", snapshot.SnapshotSdf);
        SmoothShader.SetBuffer(populateWorkBufferKernel, "colors", snapshot.SnapshotColors);
        SmoothShader.SetBuffer(populateWorkBufferKernel, "appendWorkBuffer", workBuffer);
        SmoothShader.Dispatch(populateWorkBufferKernel, snapshot.Resolution / 8, snapshot.Resolution / 8, snapshot.Resolution / 8);
        ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);

        SmoothShader.SetInt("resolution", snapshot.Resolution);
        SmoothShader.SetBuffer(applyWorkBufferKernel, "entries", countBuffer);
        SmoothShader.SetBuffer(applyWorkBufferKernel, "sdf", snapshot.SnapshotSdf);
        SmoothShader.SetBuffer(applyWorkBufferKernel, "colors", snapshot.SnapshotColors);
        SmoothShader.SetBuffer(applyWorkBufferKernel, "structuredWorkBuffer", workBuffer);
        SmoothShader.Dispatch(applyWorkBufferKernel, volume / 512, 1, 1);

        snapshot.ApplySnapshot();
    }

    private void PositionButton_OnButtonDown(XRController controller)
    {
        cursor.transform.parent = null;
    }

    private void PositionButton_OnButtonUp(XRController controller)
    {
        cursor.transform.parent = ToolController.Instance.rightController.transform;
    }
    protected override void SetMaxSize()
    {
        MaxSize = LayerManager.Instance.Spacing * 0.4f;
    }
    protected override void SetMinSize()
    {
        MinSize = LayerManager.Instance.VoxelSpacing;
    }
}
