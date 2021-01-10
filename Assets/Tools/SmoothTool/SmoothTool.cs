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
        public float avg;
        public Vector3 avgColor;
    }

    [Header("Handlers")]
    public ButtonHandler positionButton;

    public ButtonHandler upButton;
    public ButtonHandler downButton;

    [Space(10)]

    public SnapshotController snapshot;

    public AxisHandler trigger;
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

    public bool isWorking = false;
    public Dictionary<Chunk, float[]> beforeEdit;
    public Dictionary<Chunk, float[]> beforeColor;

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
        if (cursor != null)
        {
            cursor.ToggleRenderer(false);
        }
        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;
        base.Disable();
    }

    public void Start()
    {
        snapshot = GameObject.Find("Snapshot").GetComponent<SnapshotController>();
        res = LayerManager.Instance.ChunkResolution;
        volume = res * res * res;
        countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        workBuffer = new ComputeBuffer(volume, sizeof(float) * 4 + sizeof(uint) * 3, ComputeBufferType.Append);
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        cursor.gameObject.name = "SmoothCursor";
        populateWorkBufferKernel = SmoothShader.FindKernel("PopulateWorkBuffer");
        applyWorkBufferKernel = SmoothShader.FindKernel("ApplySmooth");
    }

    private void Update()
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
        SmoothShader.SetFloat("toolRadius", cursor.radius* 0.8f * (1f/snapshot.transform.localScale.x));
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
}
