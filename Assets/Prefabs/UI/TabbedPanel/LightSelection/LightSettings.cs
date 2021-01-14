using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSettings : MonoBehaviour
{
    public SceneLight sceneLight;

    public Text nameText;

    public Text rangeValue;
    public Text angleValue;

    public Dropdown dropdown;

    public Slider rangeSlider;
    public Slider angleSlider;

    public delegate void TypeChanged(LightType type);
    public TypeChanged MetallicChanged;

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

    public void SetRangeValue(float value)
    {
        sceneLight.Range = value;
        rangeValue.text = value.ToString("F2");
    }
    public void SetAngleValue(float value)
    {
        sceneLight.Angle = value;
        angleValue.text = value.ToString("F0");
    }

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
