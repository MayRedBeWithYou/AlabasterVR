﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelection : MonoBehaviour
{
    public ToolItem toolItemPrefab;

    public Transform toolSelectionBox;

    public Dictionary<Tool, ToolItem> Tools;
    void Start()
    {
        Tools = new Dictionary<Tool, ToolItem>();
        List<Tool> tools = ToolController.Instance.Tools;
        foreach(Tool tool in tools)
        {
            ToolItem toolItem = Instantiate(toolItemPrefab, toolSelectionBox);
            toolItem.tool = tool;
            toolItem.RefreshData();
            Tools.Add(tool, toolItem);
        }

        ToolController_SelectedToolChanged(ToolController.Instance.SelectedTool);

        ToolController.SelectedToolChanged += ToolController_SelectedToolChanged;
    }

    private void ToolController_SelectedToolChanged(Tool tool)
    {
        foreach(ToolItem toolItem in Tools.Values)
        {
            toolItem.HighlightItem(toolItem.tool == tool);
        }
    }
}
