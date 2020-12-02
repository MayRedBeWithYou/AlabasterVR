using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolItem : MonoBehaviour
{
    public Tool tool;

    public Text nameText;

    public Image image;

    public void OnClicked()
    {
        ToolController.Instance.SelectedTool = tool;
    }

    public void RefreshData()
    {
        nameText.text = tool.toolName;
        image.sprite = tool.sprite;
    }
}
