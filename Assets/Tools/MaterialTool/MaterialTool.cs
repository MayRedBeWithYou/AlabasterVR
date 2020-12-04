using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MaterialTool : Tool
{

    [Header("Handlers")]

    public AxisHandler trigger;
    public ButtonHandler settingsButton;
    public ButtonHandler toggleButton;
    public ButtonHandler positionButton;

    public ButtonHandler upButton;
    public ButtonHandler downButton;

    [Header("Cursor")]
    public Material addMaterial;
    public Material removeMaterial;

    [Space(10f)]
    public GameObject cursorPrefab;

    public ComputeShader addShader;
    public ComputeShader removeShader;

    int sphereShaderKernel;

    [HideInInspector]
    public CursorSDF cursor;

    public bool isAdding = true;

    public override void Enable()
    {
        if (cursor != null)
            cursor.ToggleRenderer(true);
        base.Enable();
    }

    private void PositionButton_OnButtonDown(XRController controller)
    {
        cursor.transform.parent = null;
    }

    private void PositionButton_OnButtonUp(XRController controller)
    {
        cursor.transform.parent = ToolController.Instance.rightController.transform;
    }


    public override void Disable()
    {
        cursor.ToggleRenderer(false);
        base.Disable();
    }

    public void Awake()
    {
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        toggleButton.OnButtonDown += ToggleButtonHandler;

        positionButton.OnButtonDown += PositionButton_OnButtonDown;
        positionButton.OnButtonUp += PositionButton_OnButtonUp;
    }

    private void FixedUpdate()
    {
        cursor.UpdateActiveChunks();
        if (upButton.IsPressed)
        {
            cursor.IncreaseRadius();
        }
        if (downButton.IsPressed)
        {
            cursor.DecreaseRadius();
        }
        if (trigger.Value > 0.2)
        {
            PerformAction();
        }
    }

    private void ToggleButtonHandler(XRController controller)
    {
        isAdding = !isAdding;
        cursor.SetMaterial(isAdding ? addMaterial : removeMaterial);

    }

    private void PerformAction()
    {
        ComputeShader shader = isAdding ? addShader : removeShader;
        foreach (Chunk chunk in LayerManager.Instance.ActiveLayer.chunks)
        {
            sphereShaderKernel = shader.FindKernel("CSMain");

            shader.SetFloat("radius", cursor.radius);
            shader.SetFloat("chunkSize", chunk.size);
            shader.SetVector("position", cursor.transform.position);
            shader.SetVector("offset", chunk.transform.position);
            shader.SetInt("resolution", chunk.resolution);
            shader.SetBuffer(sphereShaderKernel, "sdf", chunk.voxels.VoxelBuffer);
            shader.Dispatch(sphereShaderKernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        }

        //MeshGenerator.Instance.UpdateAllActiveChunks();
    }
}
