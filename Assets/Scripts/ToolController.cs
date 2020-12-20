using HSVPicker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolController : MonoBehaviour
{
    [Header("Controller references")]
    public GameObject rightController;
    public GameObject leftController;

    private static ToolController _instance;
    public static ToolController Instance => _instance;

    private Tool _selectedTool;

    [Space(10f)]
    public List<Tool> ToolPrefabs = new List<Tool>();

    [HideInInspector]
    public List<Tool> Tools = new List<Tool>();


    public delegate void ToolChanged(Tool tool);
    public static event ToolChanged SelectedToolChanged;

    public Tool SelectedTool
    {
        get => _selectedTool;
        set
        {
            foreach (Tool tool in Tools)
            {
                if (tool == value) tool.Enable();
                else tool.Disable();
            }
            _selectedTool = value;
            SelectedToolChanged?.Invoke(value);
            Debug.Log($"Selected tool changed to {value.name}");
        }
    }

    public void ToggleSelectedTool(bool value)
    {
        if (value) SelectedTool.Enable();
        else SelectedTool.Disable();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        foreach (Tool tool in ToolPrefabs)
        {
            Tools.Add(Instantiate(tool, transform));
        }
        SelectedTool = Tools[0];
    }
}