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
        workBuffer = new ComputeBuffer(volume, sizeof(float)*2 + sizeof(uint) * 3, ComputeBufferType.Append);
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
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
        snapshot.SetPositionReal(cursor.transform.position);

        //snapshot.SetCenter(cursor.transform.position);
        snapshot.TakeSnapshot();

        workBuffer.SetCounterValue(0);
        var kernel = SmoothShader.FindKernel("PopulateWorkBuffer");
        SmoothShader.SetFloat("toolRadius", cursor.radius);
        SmoothShader.SetFloat("chunkSize", snapshot.size);
        SmoothShader.SetVector("toolCenter", snapshot.transform.InverseTransformPoint(cursor.transform.position) + Vector3.one * snapshot.size * 0.5f);

        //SmoothShader.SetVector("toolCenter", snapshot.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
        SmoothShader.SetInt("resolution", snapshot.resolution);
        SmoothShader.SetBuffer(kernel, "sdf", snapshot.Snapshot);
        SmoothShader.SetBuffer(kernel, "appendWorkBuffer", workBuffer);
        SmoothShader.Dispatch(kernel, snapshot.resolution / 8, snapshot.resolution / 8, snapshot.resolution / 8);
        ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);

        kernel = SmoothShader.FindKernel("ApplySmooth");
        SmoothShader.SetInt("resolution", snapshot.resolution);
        SmoothShader.SetBuffer(kernel, "entries", countBuffer);
        SmoothShader.SetBuffer(kernel, "sdf", snapshot.Snapshot);
        SmoothShader.SetBuffer(kernel, "structuredWorkBuffer", workBuffer);
        SmoothShader.Dispatch(kernel, volume / 512, 1, 1);
        snapshot.ApplySnapshot();
    }
}
