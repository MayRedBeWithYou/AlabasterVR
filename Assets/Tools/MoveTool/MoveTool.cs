// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="MoveTool.cs" company="">
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
/// Class MoveTool.
/// Implements the <see cref="Tool" />
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="Tool" />
/// <seealso cref="System.IDisposable" />
public class MoveTool : Tool, IDisposable
{
    /// <summary>
    /// Struct MoveData
    /// </summary>
    public struct MoveData
    {
        /// <summary>
        /// From
        /// </summary>
        public Vector3Int from;
        /// <summary>
        /// The gradient
        /// </summary>
        public Vector3 gradient;
        /// <summary>
        /// The r gradient
        /// </summary>
        public Vector3 rGradient;
        /// <summary>
        /// The g gradient
        /// </summary>
        public Vector3 gGradient;
        /// <summary>
        /// The b gradient
        /// </summary>
        public Vector3 bGradient;
    }

    /// <summary>
    /// Up button
    /// </summary>
    [Header("Handlers")]

    public ButtonHandler upButton;
    /// <summary>
    /// Down button
    /// </summary>
    public ButtonHandler downButton;
    /// <summary>
    /// The position button
    /// </summary>
    public ButtonHandler positionButton;

    /// <summary>
    /// The trigger
    /// </summary>
    public AxisHandler trigger;

    /// <summary>
    /// The move shader
    /// </summary>
    [Header("Compute")]

    public ComputeShader MoveShader;
    int applyMoveDataKernel;
    int prepareMoveDataKernel;

    private ComputeBuffer workBuffer;
    private ComputeBuffer countBuffer;
    private SnapshotController snapshot;

    private bool workBufferPopulated;
    private Vector3 prevPos;
    private int res;
    private int volume;


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
        positionButton.OnButtonDown += PositionButton_OnButtonDown;
        positionButton.OnButtonUp += PositionButton_OnButtonUp;
        base.Enable();
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
        snapshot = GameObject.Find("Snapshot").GetComponent<SnapshotController>();

        res = LayerManager.Instance.ChunkResolution;
        volume = res * res * res;
        countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        workBuffer = new ComputeBuffer(volume, sizeof(float) * 12 + sizeof(uint) * 3, ComputeBufferType.Append);
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        InitializeShadersConstUniforms();
    }

    private void InitializeShadersConstUniforms()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        prepareMoveDataKernel = MoveShader.FindKernel("PrepareMoveData");
        applyMoveDataKernel = MoveShader.FindKernel("ApplyMoveData");

        MoveShader.SetFloat("chunkSize", snapshot.Size);
        MoveShader.SetInt("resolution", snapshot.Resolution);

        MoveShader.SetBuffer(prepareMoveDataKernel, "sdf", snapshot.SnapshotSdf);
        MoveShader.SetBuffer(prepareMoveDataKernel, "colors", snapshot.SnapshotColors);
        MoveShader.SetBuffer(prepareMoveDataKernel, "workBufferIn", workBuffer);
        MoveShader.SetBuffer(applyMoveDataKernel, "sdf", snapshot.SnapshotSdf);
        MoveShader.SetBuffer(applyMoveDataKernel, "colors", snapshot.SnapshotColors);
        MoveShader.SetBuffer(applyMoveDataKernel, "workBufferOut", workBuffer);
        MoveShader.SetBuffer(applyMoveDataKernel, "entries", countBuffer);
        MoveShader.SetFloat("voxelSpacing", snapshot.Spacing);
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

                MoveOperation op = new MoveOperation(beforeEdit, beforeColor, afterEdit, afterColor);
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
        else
        {
            workBufferPopulated = false;
        }
    }

    private void PerformAction()
    {
        if (workBufferPopulated)
        {
            ApplyWorkBuffer();
        }
        if (!workBufferPopulated)
        {
            PopulateWorkBuffer();
        }
    }

    private void PositionButton_OnButtonDown(XRController controller)
    {
        cursor.transform.parent = null;
    }

    private void PositionButton_OnButtonUp(XRController controller)
    {
        cursor.transform.parent = ToolController.Instance.rightController.transform;
    }

    private void ApplyWorkBuffer()
    {
        snapshot.SetPositionReal(prevPos);
        MoveShader.SetVector("offset", snapshot.transform.InverseTransformDirection(cursor.transform.position - prevPos));
        MoveShader.Dispatch(applyMoveDataKernel, volume/512 ,1,1);
        snapshot.ApplySnapshot();
        workBufferPopulated = false;
    }

    private void PopulateWorkBuffer()
    {
        snapshot.SetPositionReal(cursor.transform.position);
        snapshot.TakeSnapshot();

        workBuffer.SetCounterValue(0);
        MoveShader.SetFloat("toolRadius", cursor.Size * (1f / snapshot.transform.localScale.x));
        MoveShader.SetVector("toolCenter", snapshot.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position) + Vector3.one * snapshot.Size * 0.5f);
        MoveShader.Dispatch(prepareMoveDataKernel, snapshot.Resolution / 8, snapshot.Resolution / 8, snapshot.Resolution / 8);
        prevPos = cursor.transform.position;
        ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);
        workBufferPopulated = true;
    }
    protected override void SetMaxSize()
    {
        MaxSize = LayerManager.Instance.Spacing * 0.4f;
    }

    protected override void SetMinSize()
    {
        MinSize = LayerManager.Instance.VoxelSpacing;
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        if (workBuffer != null) workBuffer.Dispose();
        if (countBuffer != null) countBuffer.Dispose();
    }

    ~MoveTool()
    {
        Dispose();
    }
}
