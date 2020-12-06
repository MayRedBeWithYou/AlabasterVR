using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSelection : MonoBehaviour
{
    public GameObject contentHolder;

    public GameObject layerItemPrefab;

    public Dictionary<Layer, LayerItem> layers;

    public void Awake()
    {
        layers = new Dictionary<Layer, LayerItem>();

        foreach (Layer layer in LayerManager.Instance.layers)
        {
            GameObject go = Instantiate(layerItemPrefab, contentHolder.transform);
            LayerItem item = go.GetComponent<LayerItem>();
            item.Layer = layer;
            layers.Add(layer, item);
        }

        LayerManager_ActiveLayerChanged(LayerManager.Instance.ActiveLayer);

        LayerManager.ActiveLayerChanged += LayerManager_ActiveLayerChanged;
        LayerManager.LayerAdded += LayerManager_LayerAdded;
        LayerManager.LayerRemoved += LayerManager_LayerRemoved;
    }

    private void LayerManager_LayerRemoved(Layer layer)
    {
        LayerItem item = layers[layer];
        layers.Remove(layer);
        Destroy(item.gameObject);
    }

    private void LayerManager_LayerAdded(Layer layer)
    {
        GameObject go = Instantiate(layerItemPrefab, contentHolder.transform);
        LayerItem item = go.GetComponent<LayerItem>();
        item.Layer = layer;
        layers.Add(layer, item);
    }

    private void LayerManager_ActiveLayerChanged(Layer layer)
    {
        foreach (LayerItem item in layers.Values)
        {
            item.HighlightItem(item.Layer == layer);
        }
    }

    public void AddNewLayer()
    {
        LayerManager.Instance.AddNewLayer();
    }
}
