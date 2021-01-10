using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightItem : MonoBehaviour
{
    private SceneLight _sceneLight;
    public Text textBox;

    [SerializeField]
    private Button _toggleButton;

    [SerializeField]
    private Button _colorButton;

    [SerializeField]
    private Button _removeButton;

    private Image _image;

    private ColorPicker picker;

    public SceneLight Light
    {
        get => _sceneLight;
        set
        {
            _sceneLight = value;
            textBox.text = _sceneLight.name;
        }
    }

    public void Awake()
    {
        _toggleButton.onClick.AddListener(ToggleLight);
        _colorButton.onClick.AddListener(ChangeColor);
        _removeButton.onClick.AddListener(RemoveLight);
    }

    public void ToggleLight()
    {
        _sceneLight.ToggleLight(!_sceneLight.Enabled);
        SetToggleIcon();
    }

    public void SetToggleIcon()
    {
        if (_sceneLight.Enabled) _toggleButton.GetComponentInChildren<Image>().color = Color.white;
        else _toggleButton.GetComponentInChildren<Image>().color = Color.black;
    }
    public void ShowSettings()
    {
        LightSettings settings = UIController.Instance.ShowLightSettings();
        settings.closeButton.onClick.AddListener(() =>
        {
            TabbedPanel panel = UIController.Instance.ShowTabbedPanelMenu();
            panel.selectedTab = panel.tabButtons[1];
        });
        settings.sceneLight = _sceneLight;
    }

    public void ChangeColor()
    {
        if (picker != null) picker.Close();
        else
        {
            picker = UIController.Instance.ShowColorPicker(_sceneLight.light.color);
            LightManager.Instance.AddLightChangeListener(picker, _sceneLight);
        }
    }

    public void RemoveLight()
    {
        LightManager.Instance.RemoveLight(Light);
    }
}
