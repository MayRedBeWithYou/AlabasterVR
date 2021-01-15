// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-04-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-04-2021
// ***********************************************************************
// <copyright file="LayerSelection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class LayerSelection.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LayerSelection : MonoBehaviour
{
    /// <summary>
    /// The content holder
    /// </summary>
    public GameObject contentHolder;

    /// <summary>
    /// The layer item prefab
    /// </summary>
    public GameObject layerItemPrefab;

    /// <summary>
    /// The layers
    /// </summary>
    public Dictionary<Layer, LayerItem> layers;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        layers = new Dictionary<Layer, LayerItem>();

        foreach (Layer layer in LayerManager.Instance.layers)
        {
            LayerAdded(layer);
        }

        ActiveLayerChanged(LayerManager.Instance.ActiveLayer);

        LayerManager.ActiveLayerChanged += ActiveLayerChanged;
        LayerManager.LayerAdded += LayerAdded;
        LayerManager.LayerRemoved += LayerRemoved;
    }

    private void LayerRemoved(Layer layer)
    {
        LayerItem item = layers[layer];
        layers.Remove(layer);
        item.gameObject.SetActive(false);
        Destroy(item.gameObject);
    }

    private void LayerAdded(Layer layer)
    {
        GameObject go = Instantiate(layerItemPrefab, contentHolder.transform);
        go.name = layer.name;
        LayerItem item = go.GetComponent<LayerItem>();
        item.Layer = layer;
        item.GetComponent<Button>().onClick.AddListener(() => item.SelectLayer());
        layers.Add(layer, item);
    }

    private void ActiveLayerChanged(Layer layer)
    {
        foreach (LayerItem item in layers.Values)
        {
            item.HighlightItem(item.Layer == layer);
        }
    }

    /// <summary>
    /// Called when [destroy].
    /// </summary>
    public void OnDestroy()
    {
        LayerManager.ActiveLayerChanged -= ActiveLayerChanged;
        LayerManager.LayerAdded -= LayerAdded;
        LayerManager.LayerRemoved -= LayerRemoved;
    }
}
