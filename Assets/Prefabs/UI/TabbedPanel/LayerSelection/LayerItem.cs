// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="LayerItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class LayerItem.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LayerItem : MonoBehaviour
{
    private Layer _layer;
    /// <summary>
    /// The text box
    /// </summary>
    public Text textBox;

    [SerializeField]
    private Button _renameButton;

    private Image _image;

    /// <summary>
    /// Gets or sets the layer.
    /// </summary>
    /// <value>The layer.</value>
    public Layer Layer
    {
        get => _layer;
        set
        {
            _layer = value;
            textBox.text = _layer.name;
        }
    }

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    public void Awake()
    {
        _renameButton.onClick.AddListener(ShowKeyboard);
    }

    /// <summary>
    /// Selects the layer.
    /// </summary>
    public void SelectLayer()
    {
        LayerManager.Instance.ActiveLayer = Layer;
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
    /// Removes the layer.
    /// </summary>
    public void RemoveLayer()
    {
        LayerManager.Instance.RemoveLayer(Layer);
    }

    /// <summary>
    /// Shows the settings.
    /// </summary>
    public void ShowSettings()
    {
        LayerSettings settings = UIController.Instance.ShowLayerSettings();
        settings.closeButton.onClick.AddListener(() => UIController.Instance.ShowTabbedPanelMenu());
        settings.layer = _layer;
    }

    /// <summary>
    /// Shows the keyboard.
    /// </summary>
    public void ShowKeyboard()
    {
        Keyboard keyboard = Keyboard.Show(Layer.name);
        keyboard.OnAccepted += (text) =>
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            Layer.name = text;
            textBox.text = text;
            keyboard.Close();
        };
        keyboard.OnCancelled += () => keyboard.Close();
    }
}
