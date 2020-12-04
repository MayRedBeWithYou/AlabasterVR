using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerItem : MonoBehaviour
{
    private Layer _layer;
    public Text textBox;

    private Image _image;

    public Layer Layer
    {
        get => _layer;
        set
        {
            _layer = value;
            textBox.text = _layer.name;
        }
    }

    public void Awake()
    {
    }

    public void SelectLayer()
    {
        LayerManager.Instance.ActiveLayer = Layer;
    }

    public void HighlightItem(bool highlight)
    {
        if (highlight) GetComponent<Image>().color = Color.white;
        else GetComponent<Image>().color = new Color(Color.white.r, Color.white.g, Color.white.b, 100f/255f);
    }

    public void RemoveLayer()
    {
        LayerManager.Instance.RemoveLayer(Layer);
    }
}
