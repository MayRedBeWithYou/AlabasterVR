using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrinderTool : Tool
{

    [Header("Handlers")]

    public AxisHandler trigger;
    public ButtonHandler settingsButton;
    public ButtonHandler toggleButton;
    public ButtonHandler positionButton;

    public ButtonHandler upButton;
    public ButtonHandler downButton;

    [Header("Cursor")]
    public Color addMaterialColor;
    public Color removeMaterialColor;

    public ComputeShader shader;
    private int AddMaterialKernel;
    private int RemoveMaterialKernel;

    private ColorPicker activeColorPicker;

    public Color color;

    //[HideInInspector]
    //public SphericalCursor cursor;

    public bool isAdding = true;

    public bool isWorking = false;

    public Dictionary<Chunk, float[]> beforeEdit;

    public Dictionary<Chunk, float[]> beforeColor;

    public override void Enable()
    {
        if (!gameObject.activeSelf)
        {
            settingsButton.OnButtonDown += SettingsButton_OnButtonDown;
            toggleButton.OnButtonDown += ToggleButtonHandler;

            positionButton.OnButtonDown += PositionButton_OnButtonDown;
            positionButton.OnButtonUp += PositionButton_OnButtonUp;
            base.Enable();
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


    public override void Disable()
    {
        settingsButton.OnButtonDown -= SettingsButton_OnButtonDown;

        toggleButton.OnButtonDown -= ToggleButtonHandler;
        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;

        if (activeColorPicker != null && ToolController.Instance.SelectedTool != this) activeColorPicker.Close();
        base.Disable();
    }

    protected override void Awake()
    {
        base.Awake();
        SyncCursorColor();
    }
    protected override void Start()
    {
        base.Start();
        AddMaterialKernel = shader.FindKernel("AddMaterial");
        RemoveMaterialKernel = shader.FindKernel("RemoveMaterial");
        InitializeShadersConstUniforms();
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
            activeColorPicker = UIController.Instance.ShowColorPicker(color);
            activeColorPicker.onValueChanged.AddListener((c) => color = c);
        }
    }

    private void Update()
    {
        if (upButton.IsPressed)
        {
            cursor.IncreaseSize();
        }
        if (downButton.IsPressed)
        {
            cursor.DecreaseSize();
        }
        if (isWorking)
        {
            cursor.UpdateActiveChunks();
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

                MaterialOperation op = new MaterialOperation(beforeEdit, beforeColor, afterEdit, afterColor);
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
    private void SyncCursorColor()
    {
        cursor.Color = isAdding ? new Color(0, 1, 0, 1) : new Color(1, 0, 0, 1);
    }

    private void ToggleButtonHandler(XRController controller)
    {
        isAdding = !isAdding;
        SyncCursorColor();
    }
    private void InitializeShadersConstUniforms()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        shader.SetFloat("chunkSize", activeLayer.Spacing);
        shader.SetInt("resolution", activeLayer.ChunkResolution);
        shader.SetFloat("voxelSpacing", LayerManager.Instance.VoxelSpacing);
        shader.SetFloat("radius", cursor.Size * (1f / activeLayer.transform.localScale.x)); //Layer scale is always uniform in all dimensions, so it does not matter which component of localScale we take.
    }

    private void PerformAction()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        int kernel = isAdding ? AddMaterialKernel : RemoveMaterialKernel;
        shader.SetFloat("radius", cursor.Size * (1f));// / activeLayer.transform.localScale.x)); //Layer scale is always uniform in all dimensions, so it does not matter which component of localScale we take.
        shader.SetVector("color", new Vector3(color.r, color.g, color.b));
        var cursorMat = cursor.transform.worldToLocalMatrix;
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            Matrix4x4 voxelToCursorCoords = cursorMat * chunk.transform.localToWorldMatrix;
            shader.SetMatrix("voxelToCursorCoords", voxelToCursorCoords);
            shader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            shader.SetBuffer(kernel, "colors", chunk.voxels.ColorBuffer);
            shader.Dispatch(kernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        }
    }

    protected override void SetMaxSize()
    {
        MaxSize = LayerManager.Instance.Spacing;
    }

    protected override void SetMinSize()
    {
        MinSize = LayerManager.Instance.VoxelSpacing;
    }
}
