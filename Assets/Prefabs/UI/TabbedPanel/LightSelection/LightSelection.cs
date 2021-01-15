// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="LightSelection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class LightSelection.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LightSelection : MonoBehaviour
{
    /// <summary>
    /// The content holder
    /// </summary>
    public GameObject contentHolder;

    /// <summary>
    /// The light item prefab
    /// </summary>
    public GameObject lightItemPrefab;

    /// <summary>
    /// The lights
    /// </summary>
    public Dictionary<SceneLight, LightItem> lights;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        lights = new Dictionary<SceneLight, LightItem>();

        foreach (SceneLight light in LightManager.Instance.lights)
        {
            LightAdded(light);
        }

        LightManager.LightAdded += LightAdded;
        LightManager.LightRemoved += LightRemoved;
    }

    private void LightRemoved(SceneLight layer)
    {
        LightItem item = lights[layer];
        lights.Remove(layer);
        item.gameObject.SetActive(false);
        Destroy(item.gameObject);
    }

    private void LightAdded(SceneLight light)
    {
        GameObject go = Instantiate(lightItemPrefab, contentHolder.transform);
        go.name = light.name;
        LightItem item = go.GetComponent<LightItem>();
        item.Light = light;
        item.SetToggleIcon();
        lights.Add(light, item);
    }

    /// <summary>
    /// Called when [destroy].
    /// </summary>
    public void OnDestroy()
    {
        LightManager.LightAdded -= LightAdded;
        LightManager.LightRemoved -= LightRemoved;
    }
}
