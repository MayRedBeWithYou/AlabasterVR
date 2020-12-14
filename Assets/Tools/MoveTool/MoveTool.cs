using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTool : Tool
{
    public AxisHandler Trigger;
    public ComputeShader MoveShader;
    public GameObject cursorPrefab;

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

    public void Awake()
    {
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
        //foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        //{
        //    sphereShaderKernel = shader.FindKernel("CSMain");
        //    shader.SetFloat("radius", cursor.radius);
        //    shader.SetFloat("chunkSize", chunk.size);
        //    shader.SetVector("position", cursor.transform.position);
        //    shader.SetMatrix("modelMatrix", chunk.transform.localToWorldMatrix);
        //    shader.SetInt("resolution", chunk.resolution);
        //    shader.SetBuffer(sphereShaderKernel, "sdf", chunk.voxels.VoxelBuffer);
        //    shader.Dispatch(sphereShaderKernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
        //    chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        //}
    }
}
