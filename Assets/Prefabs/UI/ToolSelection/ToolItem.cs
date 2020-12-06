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
        ToolController.Instance.ToggleSelectedTool(false);
    }

    public void HighlightItem(bool highlight)
    {
        if (highlight) GetComponent<Image>().color = Color.white;
        else GetComponent<Image>().color = new Color(Color.white.r, Color.white.g, Color.white.b, 100f / 255f);
    }

    public void RefreshData()
    {
        nameText.text = tool.toolName;
        image.sprite = tool.sprite;
    }
}
