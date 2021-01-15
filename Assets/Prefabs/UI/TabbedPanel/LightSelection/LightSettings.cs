// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="LightSettings.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class LightSettings.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LightSettings : MonoBehaviour
{
    /// <summary>
    /// The scene light
    /// </summary>
    public SceneLight sceneLight;

    /// <summary>
    /// The name text
    /// </summary>
    public Text nameText;

    /// <summary>
    /// The range value
    /// </summary>
    public Text rangeValue;
    /// <summary>
    /// The angle value
    /// </summary>
    public Text angleValue;

    /// <summary>
    /// The dropdown
    /// </summary>
    public Dropdown dropdown;

    /// <summary>
    /// The range slider
    /// </summary>
    public Slider rangeSlider;
    /// <summary>
    /// The angle slider
    /// </summary>
    public Slider angleSlider;

    /// <summary>
    /// Delegate TypeChanged
    /// </summary>
    /// <param name="type">The type.</param>
    public delegate void TypeChanged(LightType type);
    /// <summary>
    /// The metallic changed
    /// </summary>
    public TypeChanged MetallicChanged;

    /// <summary>
    /// The close button
    /// </summary>
    public Button closeButton;

    private void Start()
    {
        nameText.text = sceneLight.name;
        dropdown.value = sceneLight.light.type == LightType.Directional ? 0 : 1;

        dropdown.onValueChanged.AddListener(SetType);

        rangeSlider.value = sceneLight.Range;
        angleSlider.value = sceneLight.Angle;

        rangeSlider.onValueChanged.AddListener(SetRangeValue);
        angleSlider.onValueChanged.AddListener(SetAngleValue);

        rangeValue.text = sceneLight.Range.ToString("F2");
        angleValue.text = sceneLight.Range.ToString("F0");
        ToggleSliders();
    }

    /// <summary>
    /// Toggles the sliders.
    /// </summary>
    public void ToggleSliders()
    {
        if(sceneLight.light.type == LightType.Directional)
        {
            rangeSlider.interactable = false;
            rangeSlider.interactable = false;
        }
        else
        {
            rangeSlider.interactable = true;
            angleSlider.interactable = true;
        }
    }

    /// <summary>
    /// Sets the range value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetRangeValue(float value)
    {
        sceneLight.Range = value;
        rangeValue.text = value.ToString("F2");
    }
    /// <summary>
    /// Sets the angle value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetAngleValue(float value)
    {
        sceneLight.Angle = value;
        angleValue.text = value.ToString("F0");
    }

    /// <summary>
    /// Sets the type.
    /// </summary>
    /// <param name="type">The type.</param>
    public void SetType(int type)
    {
        switch (type)
        {
            case 0:
                sceneLight.light.type = LightType.Directional;
                break;
            case 1:
                sceneLight.light.type = LightType.Spot;
                break;
        }
        ToggleSliders();
    }
}
