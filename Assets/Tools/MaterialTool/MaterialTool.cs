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

    public ComputeShader shader;
    private int AddMaterialKernel;
    private int RemoveMaterialKernel;

    private ColorPicker activeColorPicker;

    public Color color;

    [HideInInspector]
    public CursorSDF cursor;

    public bool isAdding = true;

    public bool isWorking = false;

    public Dictionary<Chunk, float[]> beforeEdit;

    public Dictionary<Chunk, float[]> beforeColor;

    public override void Enable()
    {
        if (!gameObject.activeSelf)
        {
            if (cursor != null)
                cursor.ToggleRenderer(true);
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
        cursor.ToggleRenderer(false);
        settingsButton.OnButtonDown -= SettingsButton_OnButtonDown;

        toggleButton.OnButtonDown -= ToggleButtonHandler;
        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;

        if (activeColorPicker != null && ToolController.Instance.SelectedTool != this) activeColorPicker.Close();
        base.Disable();
    }

    public void Awake()
    {
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        cursor.gameObject.name = "MaterialCursor";
    }
    public void Start()
    {
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
    IEnumerator RotateLayer(Layer layer, Vector3 point, Vector3 axis)
    {
        while(true)
        {
            layer.transform.RotateAround(point, axis, 1.0f);
            yield return new WaitForSeconds(0.016f);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            var coroutine = RotateLayer(LayerManager.Instance.ActiveLayer, transform.position, cursor.transform.position - transform.position);
            StartCoroutine(coroutine);
        }
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

    private void ToggleButtonHandler(XRController controller)
    {
        isAdding = !isAdding;
        cursor.SetMaterial(isAdding ? addMaterial : removeMaterial);

    }
    private void InitializeShadersConstUniforms()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        shader.SetFloat("chunkSize", activeLayer.Spacing);
        shader.SetInt("resolution", activeLayer.ChunkResolution);
        shader.SetFloat("voxelSpacing", LayerManager.Instance.VoxelSpacing);
        shader.SetFloat("radius", cursor.radius * (1f / activeLayer.transform.localScale.x)); //Scale is always uniform in all dimensions, so it does not matter which component of localScale we take.
    }

    private void PerformAction()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        int kernel = isAdding ? AddMaterialKernel : RemoveMaterialKernel;
        shader.SetFloat("radius", cursor.radius * (1f / activeLayer.transform.localScale.x)); //Scale is always uniform in all dimensions, so it does not matter which component of localScale we take.
        shader.SetVector("color", new Vector3(color.r, color.g, color.b));
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            shader.SetVector("position", chunk.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
            shader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            shader.SetBuffer(kernel, "colors", chunk.voxels.ColorBuffer);
            shader.Dispatch(kernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
            chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        }
    }
}
