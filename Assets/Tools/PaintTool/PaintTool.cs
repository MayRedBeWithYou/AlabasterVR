using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintTool : Tool
{

    [Header("Handlers")]

    public AxisHandler trigger;
    public ButtonHandler colorButton;
    public ButtonHandler upButton;
    public ButtonHandler downButton;
    public ButtonHandler positionButton;

    private ColorPicker activeColorPicker;


    [Header("Parameters")]
    public Color color;

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

        colorButton.OnButtonDown += ColorButton_OnColorDown;

        positionButton.OnButtonDown += PositionButton_OnButtonDown;
        positionButton.OnButtonUp += PositionButton_OnButtonUp;
        base.Enable();
    }

    public override void Disable()
    {
        cursor.ToggleRenderer(false);
        colorButton.OnButtonDown -= ColorButton_OnColorDown;

        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;
        base.Disable();
    }

    public void Awake()
    {
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        cursor.gameObject.name = "PaintCursor";
    }

    private void FixedUpdate()
    {
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
            cursor.UpdateActiveChunks();
            PerformAction();
        }
    }

    private void PositionButton_OnButtonDown(XRController controller)
    {
        cursor.transform.parent = null;
    }

    private void PositionButton_OnButtonUp(XRController controller)
    {
        cursor.transform.parent = ToolController.Instance.rightController.transform;
    }

    private void ColorButton_OnColorDown(XRController controller)
    {
        if (activeColorPicker != null)
        {
            activeColorPicker.gameObject.SetActive(false);
            Destroy(activeColorPicker.gameObject);
            activeColorPicker = null;
        }
        else
        {
            activeColorPicker = UIController.Instance.ShowColorPicker(color);
            activeColorPicker.onValueChanged.AddListener((c) => color = c);
        }
    }

    private void PerformAction()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        shader.SetFloat("chunkSize", activeLayer.Spacing);
        shader.SetInt("resolution", activeLayer.ChunkResolution);
        shader.SetFloat("radius", cursor.radius * (1f / activeLayer.transform.localScale.x)); //Scale is uniform in all directions, so it does not matter which component of vector we take.
        shader.SetVector("color", new Vector3(color.r, color.g, color.b));
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
