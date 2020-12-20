using HSVPicker;
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

    private ColorPicker activeColorPicker;

    public Color color = Color.white;

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

        settingsButton.OnButtonDown += SettingsButton_OnButtonDown;

        positionButton.OnButtonDown += PositionButton_OnButtonDown;
        positionButton.OnButtonUp += PositionButton_OnButtonUp;
    }

    private void SettingsButton_OnButtonDown(XRController controller)
    {
        if (activeColorPicker != null)
        {
            activeColorPicker.gameObject.SetActive(false);
            Destroy(activeColorPicker.gameObject);
            activeColorPicker = null;
        }
        else
        {
            activeColorPicker = UIController.Instance.ShowColorPicker();
            activeColorPicker.onValueChanged.AddListener((c) => color = c);
        }
    }

    private void Update()
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
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            sphereShaderKernel = shader.FindKernel("CSMain");
            shader.SetFloat("radius", cursor.radius);
            shader.SetFloat("chunkSize", chunk.size);
            shader.SetVector("position", chunk.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
            shader.SetInt("resolution", chunk.resolution);
            shader.SetBuffer(sphereShaderKernel, "sdf", chunk.voxels.VoxelBuffer);
            shader.Dispatch(sphereShaderKernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        }
    }
}
