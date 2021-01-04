using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SmoothTool : Tool
{
    public struct SmoothData
    {
        public Vector3Int from;
        public float value;
        public float avg;
    }

    public SnapshotController snapshot;

    public AxisHandler Trigger;
    public ComputeShader SmoothShader;

    public GameObject cursorPrefab;

    private ComputeBuffer workBuffer;
    private ComputeBuffer countBuffer;

    private int res;
    private int volume;

    private int populateWorkBufferKernel;
    private int applyWorkBufferKernel;

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
        if (cursor != null)
        {
            cursor.ToggleRenderer(false);
        }
        base.Disable();
    }

    public void Start()
    {
        snapshot = GameObject.Find("Snapshot").GetComponent<SnapshotController>();
        res = LayerManager.Instance.ChunkResolution;
        volume = res * res * res;
        countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        workBuffer = new ComputeBuffer(volume, sizeof(float)*2 + sizeof(uint) * 3, ComputeBufferType.Append);
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        populateWorkBufferKernel = SmoothShader.FindKernel("PopulateWorkBuffer");
        applyWorkBufferKernel = SmoothShader.FindKernel("ApplySmooth");
    }

    private void FixedUpdate()
    {
        cursor.UpdateActiveChunks();
        if(Trigger.Value > 0.2)
        {
            PerformAction();
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
        SmoothShader.SetFloat("toolRadius", cursor.radius* 0.8f * (1f/snapshot.transform.localScale.x));
        SmoothShader.SetFloat("chunkSize", snapshot.size);
        SmoothShader.SetVector("toolCenter", snapshot.transform.InverseTransformPoint(cursor.transform.position) + Vector3.one * snapshot.size * 0.5f);

        SmoothShader.SetInt("resolution", snapshot.resolution);
        SmoothShader.SetBuffer(populateWorkBufferKernel, "sdf", snapshot.Snapshot);
        SmoothShader.SetBuffer(populateWorkBufferKernel, "appendWorkBuffer", workBuffer);
        SmoothShader.Dispatch(populateWorkBufferKernel, snapshot.resolution / 8, snapshot.resolution / 8, snapshot.resolution / 8);
        ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);

        SmoothShader.SetInt("resolution", snapshot.resolution);
        SmoothShader.SetBuffer(applyWorkBufferKernel, "entries", countBuffer);
        SmoothShader.SetBuffer(applyWorkBufferKernel, "sdf", snapshot.Snapshot);
        SmoothShader.SetBuffer(applyWorkBufferKernel, "structuredWorkBuffer", workBuffer);
        SmoothShader.Dispatch(applyWorkBufferKernel, volume / 512, 1, 1);

        snapshot.ApplySnapshot();
        //snapshot.TakeSnapshot();
        //snapshot.ApplySnapshot();

    }
}
