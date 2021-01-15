// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="SceneLight.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class SceneLight.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="IMovable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="IMovable" />
public class SceneLight : MonoBehaviour, IMovable
{
    /// <summary>
    /// The Light attached to this GameObject. (Null if there is none attached).
    /// </summary>
    /// <value>The light.</value>
    public Light light { get; private set; }

    private MeshRenderer meshRenderer;
    private float range;
    private float angle;

    /// <summary>
    /// Gets or sets the range.
    /// </summary>
    /// <value>The range.</value>
    public float Range
    {
        get => range;
        set
        {
            range = value;
            light.range = value;
        }
    }

    /// <summary>
    /// Gets or sets the angle.
    /// </summary>
    /// <value>The angle.</value>
    public float Angle
    {
        get => angle;
        set
        {
            angle = value;
            light.spotAngle = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="SceneLight" /> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    public bool Enabled => light.enabled;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    public void Awake()
    {
        light = GetComponent<Light>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        range = light.range;
        angle = light.spotAngle;
    }

    /// <summary>
    /// Toggles the light.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public void ToggleLight(bool value)
    {
        if (value)
        {
            light.enabled = true;
            meshRenderer.materials[1].color = light.color;
            meshRenderer.materials[1].SetColor("_EmissionColor", light.color);
        }
        else
        {
            light.enabled = false;
            meshRenderer.materials[1].color = Color.black;
            meshRenderer.materials[1].SetColor("_EmissionColor", Color.black);
        }
    }

    /// <summary>
    /// Sets the color.
    /// </summary>
    /// <param name="color">The color.</param>
    public void SetColor(Color color)
    {
        light.color = color;
        meshRenderer.materials[1].color = color;
        meshRenderer.materials[1].SetColor("_EmissionColor", color);
    }

    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="pos">The position.</param>
    public void SetPosition(Vector3 pos)
    {
    }

    /// <summary>
    /// Sets the rotation.
    /// </summary>
    /// <param name="rot">The rot.</param>
    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }
}
