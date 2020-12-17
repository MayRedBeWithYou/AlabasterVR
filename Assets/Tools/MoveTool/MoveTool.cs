using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveTool : Tool
{
    public AxisHandler Trigger;
    public ButtonHandler Button;
    public ComputeShader MoveShader;
    public ComputeShader CopyShader;
    public ComputeShader PopulateShader;
    public ComputeShader ApplyMoveShader;

    public GameObject cursorPrefab;

    private ComputeBuffer workBuffer;
    private ComputeBuffer countBuffer;
    private ComputeBuffer debugBuffer;


    private bool workBufferPopulated;
    private Vector3 prevPos;
    private int res;
    private int volume;

    private bool triggerPressed = false;

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
        res = LayerManager.Instance.ChunkResolution;
        volume = res * res * res;
        countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        workBuffer = new ComputeBuffer(volume, sizeof(float)*2 + sizeof(uint) * 3, ComputeBufferType.Append);
        debugBuffer = new ComputeBuffer(volume, sizeof(float) + sizeof(uint) * 6, ComputeBufferType.Append);
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        Button.OnButtonDown += Button_OnButtonDown;
    }

    private void FixedUpdate()
    {
        cursor.UpdateActiveChunks();
        if(Trigger.Value > 0.2)
        {
            triggerPressed = true;
            PerformAction();
        }
        else
        {
            workBufferPopulated = false;

            triggerPressed = false;
        }
    }

    private void Button_OnButtonDown(XRController controller)
    {
        //var cpuData = new DebugOutData[volume];
        //debugBuffer.GetData(cpuData);
        //foreach (var item in cpuData)
        //{
        //    Debug.Log($"{item.from}, {item.to}, {item.value}");
        //}
    }

    private void PerformAction()
    {
        if(workBufferPopulated)
        {
            ApplyWorkBuffer();
        }
        if(workBufferPopulated == false)
        {
            PopulateWorkBuffer();
        }
    }

    public struct MoveData
    {
        public Vector3Int from;
        public float value;
        public float avg;
    }

    public struct DebugOutData
    {
        public Vector3Int from;
        public Vector3Int to;
        public float value;
    }



    private void ApplyWorkBuffer()
    {
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            Debug.Log("wszsedlem");
            debugBuffer.SetCounterValue(0);
            workBufferPopulated = false;

            var kernel = ApplyMoveShader.FindKernel("CSMain");
            ApplyMoveShader.SetFloat("spacing", chunk.size / (res - 1));
            ApplyMoveShader.SetVector("offset", cursor.transform.position - prevPos);
            ApplyMoveShader.SetInt("resolution", chunk.resolution);
            ApplyMoveShader.SetBuffer(kernel, "entries", countBuffer);
            ApplyMoveShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            ApplyMoveShader.SetBuffer(kernel, "workBuffer", workBuffer);
            ApplyMoveShader.SetBuffer(kernel, "debugBuffer", debugBuffer);
            ApplyMoveShader.Dispatch(kernel, volume/512 ,1,1);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
            break;
        }
    }

    private void PopulateWorkBuffer()
    {

        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            Debug.Log("Populatewszedlem");
            workBuffer.SetCounterValue(0);
            var kernel = PopulateShader.FindKernel("CSMain");
            PopulateShader.SetFloat("toolRadius", cursor.radius);
            PopulateShader.SetFloat("chunkSize", chunk.size);
            PopulateShader.SetVector("toolCenter", chunk.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
            PopulateShader.SetInt("resolution", chunk.resolution);
            PopulateShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            PopulateShader.SetBuffer(kernel, "workBuffer", workBuffer);
            PopulateShader.Dispatch(kernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
            //MoveData[] cpuData = new MoveData[volume];
            //workBuffer.GetData(cpuData);
            //foreach (MoveData item in cpuData)
            //{
            //    Debug.Log($"{item.from}, {item.value}");
            //}
            prevPos = cursor.transform.position;
            ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);
            int[] oneIntCpu = new int[1];
            countBuffer.GetData(oneIntCpu);
            Debug.Log($"Got {oneIntCpu[0]} elements");
            workBufferPopulated = true;
            break;
        }
    }
}
