// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-08-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-08-2021
// ***********************************************************************
// <copyright file="TabbedPanel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class TabbedPanel.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class TabbedPanel : MonoBehaviour
{
    /// <summary>
    /// The tab buttons
    /// </summary>
    public List<TabButton> tabButtons;
    /// <summary>
    /// The hovered color
    /// </summary>
    public Color hoveredColor;
    /// <summary>
    /// The selected color
    /// </summary>
    public Color selectedColor;
    /// <summary>
    /// The idle color
    /// </summary>
    public Color idleColor;

    [SerializeField]
    private TabButton _selectedTab;

    /// <summary>
    /// Gets or sets the selected tab.
    /// </summary>
    /// <value>The selected tab.</value>
    public TabButton selectedTab
    {
        get => _selectedTab;
        set
        {
            _selectedTab = value;
            ResetTabs();
        }
    }


    /// <summary>
    /// Subscribes the specified button.
    /// </summary>
    /// <param name="button">The button.</param>
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
        if (tabButtons.Count == 1)
        {
            selectedTab = button;
        }
        else ResetTabs();
    }

    /// <summary>
    /// Called when [tab enter].
    /// </summary>
    /// <param name="button">The button.</param>
    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        button.background.color = hoveredColor;
    }
    /// <summary>
    /// Called when [tab exit].
    /// </summary>
    /// <param name="button">The button.</param>
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }
    /// <summary>
    /// Called when [tab selected].
    /// </summary>
    /// <param name="button">The button.</param>
    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
    }

    /// <summary>
    /// Resets the tabs.
    /// </summary>
    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            button.background.color = idleColor;
            button.panel.SetActive(false);
        }
        selectedTab.background.color = selectedColor;
        selectedTab.panel.SetActive(true);
    }

    /// <summary>
    /// Adds the new layer.
    /// </summary>
    public void AddNewLayer()
    {
        LayerManager.Instance.AddNewLayer();
        selectedTab = tabButtons[0];
    }

    /// <summary>
    /// Adds the new light.
    /// </summary>
    public void AddNewLight()
    {
        LightManager.Instance.CreateLight();
        selectedTab = tabButtons[1];
    }

    /// <summary>
    /// Adds the new image.
    /// </summary>
    public void AddNewImage()
    {
        RefPictureManager.Instance.AddRefPicture();
        selectedTab = tabButtons[2];
    }

}
