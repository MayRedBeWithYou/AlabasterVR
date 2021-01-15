// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-07-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-10-2021
// ***********************************************************************
// <copyright file="LightManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class LightManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LightManager : MonoBehaviour
{
    private static LightManager _instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static LightManager Instance => _instance;

    /// <summary>
    /// The light holder
    /// </summary>
    [Header("References")]

    public GameObject LightHolder;

    /// <summary>
    /// The light prefab
    /// </summary>
    public GameObject LightPrefab;

    /// <summary>
    /// Gets the lights.
    /// </summary>
    /// <value>The lights.</value>
    public List<SceneLight> lights { get; private set; }


    /// <summary>
    /// Delegate LightChanged
    /// </summary>
    /// <param name="light">The light.</param>
    public delegate void LightChanged(SceneLight light);

    /// <summary>
    /// Occurs when [light added].
    /// </summary>
    public static event LightChanged LightAdded;

    /// <summary>
    /// Occurs when [light removed].
    /// </summary>
    public static event LightChanged LightRemoved;

    private int lightCount = 1;

    /// <summary>
    /// The distance
    /// </summary>
    public float distance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        lights = new List<SceneLight>();

        SceneLight light = Instantiate(LightPrefab, LightHolder.transform).GetComponent<SceneLight>();
        light.name = $"Main light";

        light.transform.position = new Vector3(0, 3, 0);
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        light.light.intensity = 1;

        light.SetColor(light.light.color);
        lights.Add(light);
    }

    /// <summary>
    /// Creates the light.
    /// </summary>
    /// <returns>SceneLight.</returns>
    public SceneLight CreateLight()
    {
        SceneLight light = Instantiate(LightPrefab, LightHolder.transform).GetComponent<SceneLight>();
        light.name = $"Light {lightCount++}";

        Vector3 lookDirection = Camera.main.transform.forward;

        light.transform.position = Camera.main.transform.position + lookDirection.normalized * distance;
        light.transform.rotation = Quaternion.LookRotation(-lookDirection, Vector3.up);

        light.SetColor(light.light.color);
        lights.Add(light);
        LightAdded?.Invoke(light);
        return light;
    }

    /// <summary>
    /// Adds the light change listener.
    /// </summary>
    /// <param name="picker">The picker.</param>
    /// <param name="light">The light.</param>
    public void AddLightChangeListener(ColorPicker picker, SceneLight light)
    {
        picker.onValueChanged.AddListener((c) => light.SetColor(c));
    }

    /// <summary>
    /// Removes the light.
    /// </summary>
    /// <param name="light">The light.</param>
    public void RemoveLight(SceneLight light)
    {
        lights.Remove(light);
        light.gameObject.SetActive(false);
        Destroy(light.gameObject);
        LightRemoved?.Invoke(light);
    }
}
