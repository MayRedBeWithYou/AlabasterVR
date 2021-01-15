// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="LayerSettings.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class LayerSettings.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LayerSettings : MonoBehaviour
{
    /// <summary>
    /// The layer
    /// </summary>
    public Layer layer;

    /// <summary>
    /// The metallic slider
    /// </summary>
    public Slider metallicSlider;
    /// <summary>
    /// The smoothness slider
    /// </summary>
    public Slider smoothnessSlider;

    /// <summary>
    /// The name text
    /// </summary>
    public Text nameText;

    /// <summary>
    /// The metallic value
    /// </summary>
    public Text metallicValue;
    /// <summary>
    /// The smoothness value
    /// </summary>
    public Text smoothnessValue;

    /// <summary>
    /// The render dropdown
    /// </summary>
    public Dropdown renderDropdown;

    /// <summary>
    /// Delegate ValueChanged
    /// </summary>
    /// <param name="value">The value.</param>
    public delegate void ValueChanged(float value);
    /// <summary>
    /// The metallic changed
    /// </summary>
    public ValueChanged MetallicChanged;
    /// <summary>
    /// The smoothness changed
    /// </summary>
    public ValueChanged SmoothnessChanged;

    /// <summary>
    /// The close button
    /// </summary>
    public Button closeButton;

    private void Start()
    {
        metallicSlider.onValueChanged.AddListener(SetMetallicValue);
        smoothnessSlider.onValueChanged.AddListener(SetSmoothnessValue);

        nameText.text = layer.name;

        metallicValue.text = layer.Metallic.ToString();
        smoothnessValue.text = layer.Smoothness.ToString();

        metallicSlider.value = layer.Metallic;
        smoothnessSlider.value = layer.Smoothness;

        renderDropdown.value = (int)layer.RenderType;

        renderDropdown.onValueChanged.AddListener(SetRenderType);
    }

    /// <summary>
    /// Sets the metallic value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetMetallicValue(float value)
    {
        layer.Metallic = value;
        metallicValue.text = value.ToString("F3");
    }
    /// <summary>
    /// Sets the smoothness value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetSmoothnessValue(float value)
    {
        layer.Smoothness = value;
        smoothnessValue.text = value.ToString("F3");
    }

    /// <summary>
    /// Sets the type of the render.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetRenderType(int value)
    {
        layer.RenderType = (RenderType)value;
    }

}
