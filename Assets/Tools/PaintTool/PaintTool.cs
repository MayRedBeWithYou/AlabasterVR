using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintTool : Tool
{

    [Header("Handlers")]

    public AxisHandler trigger;


    [Space(10f)]
    public GameObject cursorPrefab;

    public ComputeShader shader;

    int shaderKernel;

    [HideInInspector]
    public CursorSDF cursor;

    public override void Enable()
    {
        if (cursor != null)
            cursor.ToggleRenderer(true);
        base.Enable();
    }

    public override void Disable()
    {
        cursor.ToggleRenderer(false);
        base.Disable();
    }

    public void Awake()
    {
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
    }

    private void FixedUpdate()
    {
        if (trigger.Value > 0.2)
        {
            cursor.UpdateActiveChunks();
            PerformAction();
        }
    }


    private void PerformAction()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        shader.SetFloat("chunkSize", activeLayer.Spacing);
        shader.SetInt("resolution", activeLayer.ChunkResolution);
        shader.SetFloat("radius", cursor.radius * (1f / activeLayer.transform.localScale.x)); //Scale is uniform in all directions, so it does not matter which component of vector we take.
        shader.SetVector("color", new Vector3(0.0f, 0.0f, 1.0f));
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            shaderKernel = shader.FindKernel("CSMain");
            shader.SetVector("position", chunk.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
            shader.SetBuffer(shaderKernel, "colors", chunk.voxels.ColorBuffer);
            shader.Dispatch(shaderKernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        }
    }
}
