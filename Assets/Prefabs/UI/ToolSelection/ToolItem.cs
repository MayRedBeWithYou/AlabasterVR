// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-06-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-06-2020
// ***********************************************************************
// <copyright file="ToolItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class ToolItem.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ToolItem : MonoBehaviour
{
    /// <summary>
    /// The tool
    /// </summary>
    public Tool tool;

    /// <summary>
    /// The name text
    /// </summary>
    public Text nameText;

    /// <summary>
    /// The image
    /// </summary>
    public Image image;

    /// <summary>
    /// Called when [clicked].
    /// </summary>
    public void OnClicked()
    {
        ToolController.Instance.SelectedTool = tool;
        ToolController.Instance.ToggleSelectedTool(false);
    }

    /// <summary>
    /// Highlights the item.
    /// </summary>
    /// <param name="highlight">if set to <c>true</c> [highlight].</param>
    public void HighlightItem(bool highlight)
    {
        if (highlight) GetComponent<Image>().color = Color.white;
        else GetComponent<Image>().color = new Color(Color.white.r, Color.white.g, Color.white.b, 100f / 255f);
    }

    /// <summary>
    /// Refreshes the data.
    /// </summary>
    public void RefreshData()
    {
        nameText.text = tool.toolName;
        image.sprite = tool.sprite;
    }
}
