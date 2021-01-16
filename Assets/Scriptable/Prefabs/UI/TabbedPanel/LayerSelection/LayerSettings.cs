using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerSettings : MonoBehaviour
{
    public Layer layer;

    public Slider metallicSlider;
    public Slider smoothnessSlider;

    public Text nameText;

    public Text metallicValue;
    public Text smoothnessValue;

    public Dropdown renderDropdown;

    public delegate void ValueChanged(float value);
    public ValueChanged MetallicChanged;
    public ValueChanged SmoothnessChanged;

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

    public void SetMetallicValue(float value)
    {
        layer.Metallic = value;
        metallicValue.text = value.ToString("F3");
    }
    public void SetSmoothnessValue(float value)
    {
        layer.Smoothness = value;
        smoothnessValue.text = value.ToString("F3");
    }

    public void SetRenderType(int value)
    {
        layer.RenderType = (RenderType)value;
    }

}
