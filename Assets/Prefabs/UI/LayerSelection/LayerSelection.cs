using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerSelection : MonoBehaviour
{
    public GameObject contentHolder;

    public GameObject layerItemPrefab;

    public Dictionary<Layer, LayerItem> layers;

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

    public void AddNewLayer()
    {
        LayerManager.Instance.AddNewLayer();
    }

    public void OnDestroy()
    {
        LayerManager.ActiveLayerChanged -= ActiveLayerChanged;
        LayerManager.LayerAdded -= LayerAdded;
        LayerManager.LayerRemoved -= LayerRemoved;
    }
}
