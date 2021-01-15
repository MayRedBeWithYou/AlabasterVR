// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-04-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-04-2021
// ***********************************************************************
// <copyright file="ToolSelection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ToolSelection.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ToolSelection : MonoBehaviour
{
    /// <summary>
    /// The tool item prefab
    /// </summary>
    public ToolItem toolItemPrefab;

    /// <summary>
    /// The tool selection box
    /// </summary>
    public Transform toolSelectionBox;

    /// <summary>
    /// Dictionary for mapping Tools to their ToolItems.
    /// </summary>
    public Dictionary<Tool, ToolItem> Tools;

    void Start()
    {
        Tools = new Dictionary<Tool, ToolItem>();
        List<Tool> tools = ToolController.Instance.Tools;
        foreach (Tool tool in tools)
        {
            ToolItem toolItem = Instantiate(toolItemPrefab, toolSelectionBox);
            toolItem.tool = tool;
            toolItem.RefreshData();
            Tools.Add(tool, toolItem);
        }

        SelectedToolChanged(ToolController.Instance.SelectedTool);

        ToolController.SelectedToolChanged += SelectedToolChanged;
    }

    private void SelectedToolChanged(Tool tool)
    {
        foreach (ToolItem toolItem in Tools.Values)
        {
            toolItem.HighlightItem(toolItem.tool == tool);
        }
    }

    void OnDestroy()
    {
        ToolController.SelectedToolChanged -= SelectedToolChanged;
    }
}
