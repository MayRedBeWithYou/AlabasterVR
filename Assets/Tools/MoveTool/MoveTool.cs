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
    public ComputeShader MoveShader;
    public ComputeShader PopulateShader;
    public ComputeShader ApplyMoveShader;

    public GameObject cursorPrefab;

    private ComputeBuffer workBuffer;
    private ComputeBuffer countBuffer;

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
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            var kernel = ApplyMoveShader.FindKernel("CSMain");
            ApplyMoveShader.SetFloat("spacing", chunk.size / (res - 1));
            ApplyMoveShader.SetVector("offset", cursor.transform.position - prevPos);
            ApplyMoveShader.SetInt("resolution", chunk.resolution);
            ApplyMoveShader.SetBuffer(kernel, "entries", countBuffer);
            ApplyMoveShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            ApplyMoveShader.SetBuffer(kernel, "workBuffer", workBuffer);
            ApplyMoveShader.Dispatch(kernel, volume/512 ,1,1);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
            break;
        }
    }

    private void PopulateWorkBuffer()
    {

        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            workBuffer.SetCounterValue(0);
            var kernel = PopulateShader.FindKernel("CSMain");
            PopulateShader.SetFloat("toolRadius", cursor.radius);
            PopulateShader.SetFloat("chunkSize", chunk.size);
            PopulateShader.SetVector("toolCenter", chunk.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
            PopulateShader.SetInt("resolution", chunk.resolution);
            PopulateShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            PopulateShader.SetBuffer(kernel, "workBuffer", workBuffer);
            PopulateShader.Dispatch(kernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
            prevPos = cursor.transform.position;
            ComputeBuffer.CopyCount(workBuffer, countBuffer, 0);
            workBufferPopulated = true;
            break;
        }
    }
}
