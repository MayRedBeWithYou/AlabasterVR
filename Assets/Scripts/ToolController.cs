// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="ToolController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HSVPicker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class ToolController.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ToolController : MonoBehaviour
{
    /// <summary>
    /// The right controller
    /// </summary>
    [Header("Controller references")]
    public GameObject rightController;
    /// <summary>
    /// The left controller
    /// </summary>
    public GameObject leftController;

    private static ToolController _instance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ToolController Instance => _instance;

    private Tool _selectedTool;

    /// <summary>
    /// The tool prefabs
    /// </summary>
    [Space(10f)]
    public List<Tool> ToolPrefabs = new List<Tool>();

    /// <summary>
    /// The tools
    /// </summary>
    [HideInInspector]
    public List<Tool> Tools = new List<Tool>();


    /// <summary>
    /// Delegate ToolChanged
    /// </summary>
    /// <param name="tool">The tool.</param>
    public delegate void ToolChanged(Tool tool);
    /// <summary>
    /// Occurs when [selected tool changed].
    /// </summary>
    public static event ToolChanged SelectedToolChanged;

    /// <summary>
    /// Gets or sets the selected tool.
    /// </summary>
    /// <value>The selected tool.</value>
    public Tool SelectedTool
    {
        get => _selectedTool;
        set
        {
            _selectedTool = value;
            foreach (Tool tool in Tools)
            {
                if (tool == value) tool.Enable();
                else tool.Disable();
            }
            SelectedToolChanged?.Invoke(value);
            Debug.Log($"Selected tool changed to {value.name}");
        }
    }

    /// <summary>
    /// Toggles the selected tool.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
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
            Tools[Tools.Count - 1].Enable();
        }
        SelectedTool = Tools[0];
    }
}