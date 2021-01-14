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
    public ButtonHandler leftButton;
    public ButtonHandler rightButton;

    [Header("Cursor")]
    public Color addMaterialColor;
    public Color removeMaterialColor;

    private ColorPicker activeColorPicker;

    public Color color;

    public bool isAdding = true;

    public bool isWorking = false;

    public Dictionary<Chunk, float[]> beforeEdit;

    public Dictionary<Chunk, float[]> beforeColor;

    public List<MaterialCursor> materialCursorPrefabs;

    [HideInInspector]
    public List<MaterialCursor> materialCursors;

    [SerializeField]
    private MaterialCursor selectedCursor;

    public MaterialCursor SelectedCursor
    {
        get => selectedCursor;
        set
        {
            foreach (MaterialCursor cursor in materialCursors)
            {
                cursor.Disable();
            }
            selectedCursor = value;
            SyncCursorColor();
            selectedCursor.Enable();
        }
    }

    public override void Enable()
    {
        if (!gameObject.activeSelf)
        {
            settingsButton.OnButtonDown += SettingsButton_OnButtonDown;
            toggleButton.OnButtonDown += ToggleButtonHandler;

            positionButton.OnButtonDown += PositionButton_OnButtonDown;
            positionButton.OnButtonUp += PositionButton_OnButtonUp;
            leftButton.OnButtonDown += PrevCursor;
            rightButton.OnButtonDown += NextCursor;

            SelectedCursor?.Enable();
            gameObject.SetActive(true);
        }
    }

    public override void Disable()
    {
        settingsButton.OnButtonDown -= SettingsButton_OnButtonDown;

        toggleButton.OnButtonDown -= ToggleButtonHandler;
        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;
        SelectedCursor.Disable();

        if (activeColorPicker != null && ToolController.Instance.SelectedTool != this) activeColorPicker.Close();
        gameObject.SetActive(false);
    }

    private void PositionButton_OnButtonDown(XRController controller)
    {
        selectedCursor.transform.parent = null;
    }

    private void PositionButton_OnButtonUp(XRController controller)
    {
        selectedCursor.transform.parent = ToolController.Instance.rightController.transform;
    }

    protected override void Awake()
    {
        materialCursors = new List<MaterialCursor>();
        foreach (MaterialCursor cursor in materialCursorPrefabs)
        {
            MaterialCursor matCursor = Instantiate(cursor, ToolController.Instance.rightController.transform);
            materialCursors.Add(matCursor);
        }
        if (selectedCursor == null) SelectedCursor = materialCursors[0];
        SyncCursorColor();
    }
    protected override void Start()
    {
        SetMinSize();
        SetMaxSize();
        foreach (MaterialCursor cursor in materialCursors) cursor.SetSizeToDefault();
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
            SelectedCursor.IncreaseSize();
        }
        if (downButton.IsPressed)
        {
            SelectedCursor.DecreaseSize();
        }
        SelectedCursor.UpdateActiveChunks();
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
        selectedCursor.SetColor(isAdding ? addMaterialColor : removeMaterialColor);
    }

    private void ToggleButtonHandler(XRController controller)
    {
        isAdding = !isAdding;
        SyncCursorColor();
    }

    private void NextCursor(XRController controller)
    {
        int index = materialCursors.IndexOf(SelectedCursor);
        SelectedCursor = materialCursors[(index + 1) % materialCursors.Count];
    }
    private void PrevCursor(XRController controller)
    {
        int index = materialCursors.IndexOf(SelectedCursor);
        index = (index - 1) % materialCursors.Count;
        if (index < 0) index = materialCursors.Count - 1;
        SelectedCursor = materialCursors[index];
    }

    private void PerformAction()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        int kernel = isAdding ? SelectedCursor.AddMaterialKernel : SelectedCursor.RemoveMaterialKernel;
        ComputeShader shader = SelectedCursor.PrepareShader(color);
        var cursorMat = SelectedCursor.transform.worldToLocalMatrix;
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
        foreach (MaterialCursor cursor in materialCursors) cursor.SetMaximumSize(LayerManager.Instance.Spacing);
    }

    protected override void SetMinSize()
    {
        foreach (MaterialCursor cursor in materialCursors) cursor.SetMaximumSize(LayerManager.Instance.VoxelSpacing);
    }
}
