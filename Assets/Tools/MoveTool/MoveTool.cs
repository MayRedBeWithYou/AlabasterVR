﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveTool : Tool, IDisposable
{
    public struct MoveData
    {
        public Vector3Int from;
        public Vector3 gradient;
        public Vector3 rGradient;
        public Vector3 gGradient;
        public Vector3 bGradient;
    }

    [Header("Handlers")]

    public ButtonHandler upButton;
    public ButtonHandler downButton;
    public ButtonHandler positionButton;

    public AxisHandler Trigger;

    [Header("Compute")]

    public ComputeShader PopulateShader;
    public ComputeShader ApplyMoveShader;

    public GameObject cursorPrefab;

    private ComputeBuffer workBuffer;
    private ComputeBuffer countBuffer;
    private SnapshotController snapshot;

    private bool workBufferPopulated;
    private Vector3 prevPos;
    private int res;
    private int volume;

    [HideInInspector]
    public CursorSDF cursor;

    public override void Enable()
    {
        if(cursor != null)
        {
            cursor.ToggleRenderer(true);
        }
        positionButton.OnButtonDown += PositionButton_OnButtonDown;
        positionButton.OnButtonUp += PositionButton_OnButtonUp;
        base.Enable();
    }

    public override void Disable()
    {
        cursor.ToggleRenderer(false);
        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;
        base.Disable();
    }

    public void Awake()
    {
        snapshot = GameObject.Find("Snapshot").GetComponent<SnapshotController>();

        res = LayerManager.Instance.ChunkResolution;
        volume = res * res * res;
        countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        workBuffer = new ComputeBuffer(volume, sizeof(float) * 12 + sizeof(uint) * 3, ComputeBufferType.Append);
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        cursor.gameObject.name = "MoveCursor";
    }

    private void FixedUpdate()
    {
        if (upButton.IsPressed)
        {
            cursor.IncreaseRadius();
        }
        if (downButton.IsPressed)
        {
            cursor.DecreaseRadius();
        }
        cursor.UpdateActiveChunks();
        if(Trigger.Value > 0.2)
        {
            PerformAction();
        }
        else
        {
            workBufferPopulated = false;
        }
    }

    private void PerformAction()
    {
        if(workBufferPopulated)
        {
            ApplyWorkBuffer();
        }
        if(!workBufferPopulated)
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
        snapshot.SetPositionReal(prevPos);//cursor.transform.position);
        var kernel = ApplyMoveShader.FindKernel("CSMain");
        ApplyMoveShader.SetFloat("spacing", snapshot.spacing / (res - 1));
        ApplyMoveShader.SetVector("offset", snapshot.transform.InverseTransformDirection(cursor.transform.position - prevPos));
        ApplyMoveShader.SetFloat("voxelSpacing", snapshot.spacing);
        ApplyMoveShader.SetInt("resolution", snapshot.resolution);
        ApplyMoveShader.SetBuffer(kernel, "entries", countBuffer);
        ApplyMoveShader.SetBuffer(kernel, "sdf", snapshot.Snapshot);
        ApplyMoveShader.SetBuffer(kernel, "colors", snapshot.Colors);
        ApplyMoveShader.SetBuffer(kernel, "workBuffer", workBuffer);
        ApplyMoveShader.Dispatch(kernel, volume/512 ,1,1);
        snapshot.ApplySnapshot();
        workBufferPopulated = false;
        //chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
    }

    private void PopulateWorkBuffer()
    {
        snapshot.SetPositionReal(cursor.transform.position);
        snapshot.TakeSnapshot();

        workBuffer.SetCounterValue(0);
        var kernel = PopulateShader.FindKernel("CSMain");
        PopulateShader.SetFloat("toolRadius", cursor.radius * (1f / snapshot.transform.localScale.x));
        PopulateShader.SetFloat("chunkSize", snapshot.size);
        PopulateShader.SetVector("toolCenter", snapshot.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position) + Vector3.one * snapshot.size * 0.5f);
        PopulateShader.SetInt("resolution", snapshot.resolution);
        PopulateShader.SetBuffer(kernel, "sdf", snapshot.Snapshot);
        PopulateShader.SetBuffer(kernel, "colors", snapshot.Colors);
        PopulateShader.SetBuffer(kernel, "workBuffer", workBuffer);
        PopulateShader.Dispatch(kernel, snapshot.resolution / 8, snapshot.resolution / 8, snapshot.resolution / 8);
        prevPos = cursor.transform.position;
        ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);
        workBufferPopulated = true;
    }

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
