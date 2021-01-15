// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="LightItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class LightItem.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LightItem : MonoBehaviour
{
    private SceneLight _sceneLight;
    /// <summary>
    /// The text box
    /// </summary>
    public Text textBox;

    [SerializeField]
    private Button _toggleButton;

    [SerializeField]
    private Button _colorButton;

    [SerializeField]
    private Button _removeButton;

    private Image _image;

    private ColorPicker picker;

    /// <summary>
    /// Gets or sets the light.
    /// </summary>
    /// <value>The light.</value>
    public SceneLight Light
    {
        get => _sceneLight;
        set
        {
            _sceneLight = value;
            textBox.text = _sceneLight.name;
        }
    }

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    public void Awake()
    {
        _toggleButton.onClick.AddListener(ToggleLight);
        _colorButton.onClick.AddListener(ChangeColor);
        _removeButton.onClick.AddListener(RemoveLight);
    }

    /// <summary>
    /// Toggles the light.
    /// </summary>
    public void ToggleLight()
    {
        _sceneLight.ToggleLight(!_sceneLight.Enabled);
        SetToggleIcon();
    }

    /// <summary>
    /// Sets the toggle icon.
    /// </summary>
    public void SetToggleIcon()
    {
        if (_sceneLight.Enabled) _toggleButton.GetComponentInChildren<Image>().color = Color.white;
        else _toggleButton.GetComponentInChildren<Image>().color = Color.black;
    }
    /// <summary>
    /// Shows the settings.
    /// </summary>
    public void ShowSettings()
    {
        LightSettings settings = UIController.Instance.ShowLightSettings();
        settings.closeButton.onClick.AddListener(() =>
        {
            TabbedPanel panel = UIController.Instance.ShowTabbedPanelMenu();
            panel.selectedTab = panel.tabButtons[1];
        });
        settings.sceneLight = _sceneLight;
    }

    /// <summary>
    /// Changes the color.
    /// </summary>
    public void ChangeColor()
    {
        if (picker != null) picker.Close();
        else
        {
            picker = UIController.Instance.ShowColorPicker(_sceneLight.light.color);
            LightManager.Instance.AddLightChangeListener(picker, _sceneLight);
        }
    }

    /// <summary>
    /// Removes the light.
    /// </summary>
    public void RemoveLight()
    {
        LightManager.Instance.RemoveLight(Light);
    }

    void OnDestroy()
    {
        if (picker != null) picker.Close(); 
    }
}
