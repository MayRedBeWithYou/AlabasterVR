using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveTool : Tool
{
    public struct MoveData
    {
        public Vector3Int from;
        public float value;
    }


    public AxisHandler Trigger;
    //public ComputeShader MoveShader;
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
        base.Enable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    public void Start()
    {
        snapshot = GameObject.Find("Snapshot").GetComponent<SnapshotController>();

        res = LayerManager.Instance.ChunkResolution;
        volume = res * res * res;
        countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        workBuffer = new ComputeBuffer(volume, sizeof(float) + sizeof(uint) * 3, ComputeBufferType.Append);
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
    }

    private void FixedUpdate()
    {
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


    private void ApplyWorkBuffer()
    {
        snapshot.SetPositionReal(cursor.transform.position);
     
        var kernel = ApplyMoveShader.FindKernel("CSMain");
        ApplyMoveShader.SetFloat("spacing", snapshot.spacing / (res - 1));
        ApplyMoveShader.SetVector("offset", cursor.transform.position - prevPos);
        ApplyMoveShader.SetInt("resolution", snapshot.resolution);
        ApplyMoveShader.SetBuffer(kernel, "entries", countBuffer);
        ApplyMoveShader.SetBuffer(kernel, "sdf", snapshot.Snapshot);
        ApplyMoveShader.SetBuffer(kernel, "workBuffer", workBuffer);
        ApplyMoveShader.Dispatch(kernel, volume/512 ,1,1);
        snapshot.ApplySnapshot();
        //chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
    }

    private void PopulateWorkBuffer()
    {
        snapshot.SetPositionReal(cursor.transform.position);
        snapshot.TakeSnapshot();

        workBuffer.SetCounterValue(0);
        var kernel = PopulateShader.FindKernel("CSMain");
        PopulateShader.SetFloat("toolRadius", cursor.radius);
        PopulateShader.SetFloat("chunkSize", snapshot.size);
        PopulateShader.SetVector("toolCenter", snapshot.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
        PopulateShader.SetInt("resolution", snapshot.resolution);
        PopulateShader.SetBuffer(kernel, "sdf", snapshot.Snapshot);
        PopulateShader.SetBuffer(kernel, "workBuffer", workBuffer);
        PopulateShader.Dispatch(kernel, snapshot.resolution / 8, snapshot.resolution / 8, snapshot.resolution / 8);
        prevPos = cursor.transform.position;
        ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);
        workBufferPopulated = true;
    }
}
