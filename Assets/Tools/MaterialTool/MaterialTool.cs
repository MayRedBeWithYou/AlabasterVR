// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="MaterialTool.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class MaterialTool.
/// Implements the <see cref="Tool" />
/// </summary>
/// <seealso cref="Tool" />
public class MaterialTool : Tool
{

    /// <summary>
    /// The trigger
    /// </summary>
    [Header("Handlers")]

    public AxisHandler trigger;
    /// <summary>
    /// The settings button
    /// </summary>
    public ButtonHandler settingsButton;
    /// <summary>
    /// The toggle button
    /// </summary>
    public ButtonHandler toggleButton;
    /// <summary>
    /// The position button
    /// </summary>
    public ButtonHandler positionButton;

    /// <summary>
    /// Up button
    /// </summary>
    public ButtonHandler upButton;
    /// <summary>
    /// Down button
    /// </summary>
    public ButtonHandler downButton;
    /// <summary>
    /// The left button
    /// </summary>
    public ButtonHandler leftButton;
    /// <summary>
    /// The right button
    /// </summary>
    public ButtonHandler rightButton;

    /// <summary>
    /// The color for cursor while adding material
    /// </summary>
    [Header("Cursor")]
    public Color addMaterialColor;
    /// <summary>
    /// The color for cursor while removing material
    /// </summary>
    public Color removeMaterialColor;

    private ColorPicker activeColorPicker;

    /// <summary>
    /// The color of material
    /// </summary>
    public Color color;

    /// <summary>
    /// Is tool currently in add or remove mode
    /// </summary>
    public bool isAdding = true;

    /// <summary>
    /// Is tool currently working
    /// </summary>
    public bool isWorking = false;

    Dictionary<Chunk, float[]> beforeEdit;

    Dictionary<Chunk, float[]> beforeColor;

    /// <summary>
    /// The material cursor prefabs
    /// </summary>
    public List<MaterialCursor> materialCursorPrefabs;

    /// <summary>
    /// Material cursors (instantiated)
    /// </summary>
    [HideInInspector]
    public List<MaterialCursor> materialCursors;

    [SerializeField]
    private MaterialCursor selectedCursor;

    /// <summary>
    /// Gets or sets the selected cursor.
    /// </summary>
    /// <value>The selected cursor.</value>
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

    /// <summary>
    /// Enables this instance.
    /// </summary>
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

    /// <summary>
    /// Disables this instance.
    /// </summary>
    public override void Disable()
    {
        settingsButton.OnButtonDown -= SettingsButton_OnButtonDown;

        toggleButton.OnButtonDown -= ToggleButtonHandler;
        positionButton.OnButtonDown -= PositionButton_OnButtonDown;
        positionButton.OnButtonUp -= PositionButton_OnButtonUp;

        leftButton.OnButtonDown -= PrevCursor;
        rightButton.OnButtonDown -= NextCursor;
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
