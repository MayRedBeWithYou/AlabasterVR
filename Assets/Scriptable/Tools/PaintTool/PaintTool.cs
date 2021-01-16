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

    [SerializeField]
    protected Color defaultColor;
    [Header("Parameters"), SerializeField]
    protected Color _color;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            cursor.Color = value;
        }
    }

    public ComputeShader shader;

    int shaderKernel;

    public bool isWorking = false;
    public Dictionary<Chunk, float[]> beforeColor;

    public override void Enable()
    {
        if (!gameObject.activeSelf)
        {
            colorButton.OnButtonDown += ColorButton_OnColorDown;

            positionButton.OnButtonDown += PositionButton_OnButtonDown;
            positionButton.OnButtonUp += PositionButton_OnButtonUp;
            base.Enable();
        }
    }

    public override void Disable()
    {
        colorButton.OnButtonDown -= ColorButton_OnColorDown;

        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;

        if (activeColorPicker != null && ToolController.Instance.SelectedTool != this) activeColorPicker.Close();
        base.Disable();
    }

    protected override void Awake()
    {
        base.Awake();
        Color = defaultColor;
    }

    protected override void Start()
    {
        base.Start();
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
                foreach (Chunk chunk in beforeColor.Keys)
                {
                    float[] colors = new float[chunk.voxels.Volume * 3];
                    chunk.voxels.ColorBuffer.GetData(colors);
                    afterColor.Add(chunk, colors);
                }

                ColorEditOperation op = new ColorEditOperation(beforeColor, afterColor);
                OperationManager.Instance.PushOperation(op);
            }
            foreach (Chunk chunk in LayerManager.Instance.activeChunks)
            {
                if (!beforeColor.ContainsKey(chunk))
                {
                    float[] colors = new float[chunk.voxels.Volume * 3];
                    chunk.voxels.ColorBuffer.GetData(colors);
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
                beforeColor = new Dictionary<Chunk, float[]>();
            }
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
            activeColorPicker = UIController.Instance.ShowColorPicker(Color);
            activeColorPicker.onValueChanged.AddListener((c) => Color = c);
        }
    }

    private void PerformAction()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;
        shader.SetFloat("chunkSize", activeLayer.Spacing);
        shader.SetInt("resolution", activeLayer.ChunkResolution);
        shader.SetFloat("radius", cursor.Size * (1f / activeLayer.transform.localScale.x)); //Scale is uniform in all directions, so it does not matter which component of vector we take.
        shader.SetVector("color", new Vector3(Color.r, Color.g, Color.b));
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            shaderKernel = shader.FindKernel("CSMain");
            shader.SetVector("position", chunk.transform.worldToLocalMatrix.MultiplyPoint(cursor.transform.position));
            shader.SetBuffer(shaderKernel, "colors", chunk.voxels.ColorBuffer);
            shader.Dispatch(shaderKernel, chunk.resolution / 8, chunk.resolution / 8, chunk.resolution / 8);
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
