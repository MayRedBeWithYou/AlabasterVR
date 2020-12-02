using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelection : MonoBehaviour
{
    public ToolItem toolItemPrefab;

    public Transform toolSelectionBox;
    void Start()
    {
        List<Tool> tools = ToolController.Instance.Tools;
        foreach(Tool tool in tools)
        {
            ToolItem toolItem = Instantiate(toolItemPrefab, toolSelectionBox);
            toolItem.tool = tool;
            toolItem.RefreshData();
        }
    }
}
