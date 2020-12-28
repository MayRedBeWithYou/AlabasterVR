using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UIController : MonoBehaviour
{
    private static UIController _instance;
    public static UIController Instance => _instance;

    [Header("Buttons")]
    public ButtonHandler MainMenuButtonHandler;
    public ButtonHandler ToolSelectionMenuButtonHandler;
    public ButtonHandler LayerSelectionMenuButtonHandler;

    [Header("Menu parameters")]
    public Transform cameraTransform;
    public Transform LeftHandMenuTransform;

    public float uiDistance;

    [Header("Menu prefabs")]

    public GameObject MainMenuPrefab;
    public GameObject ToolSelectionMenuPrefab;
    public GameObject TabbedPanelMenuPrefab;
    public GameObject colorPickerPrefab;
    public GameObject KeyboardPrefab;

    private Keyboard _activeKeyboard = null;
    private MainMenu _activeMainMenu = null;
    private GameObject _activeLeftHandMenu = null;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        MainMenuButtonHandler.OnButtonDown += ShowMainMenu;

        ToolSelectionMenuButtonHandler.OnButtonDown += ShowToolSelectionMenu;
        LayerSelectionMenuButtonHandler.OnButtonDown += ShowLayerSelectionMenu;
    }



    private void ShowMainMenu(XRController controller)
    {
        if (_activeMainMenu)
        {
            _activeMainMenu.Close();
        }
        else
        {
            Vector3 lookDirection = cameraTransform.forward;
            lookDirection.y = 0;
            _activeMainMenu = CreateUI(MainMenuPrefab, cameraTransform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up)).GetComponent<MainMenu>();
        }
    }

    private void ShowToolSelectionMenu(XRController controller)
    {
        if (_activeLeftHandMenu)
        {
            _activeLeftHandMenu.SetActive(false);
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }
        else
        {
            _activeLeftHandMenu = CreateUI(ToolSelectionMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        }
    }

    private void ShowLayerSelectionMenu(XRController controller)
    {
        if (_activeLeftHandMenu)
        {
            _activeLeftHandMenu.SetActive(false);
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }
        else
        {
            _activeLeftHandMenu = CreateUI(TabbedPanelMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        }
    }

    public Keyboard ShowKeyboard(string text)
    {
        if (_activeKeyboard != null)
        {
            _activeKeyboard.Close();
        }

        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        GameObject go = CreateUI(KeyboardPrefab, Camera.main.transform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up));

        _activeKeyboard = go.GetComponent<Keyboard>();
        _activeKeyboard.OnClosing += () => _activeKeyboard = null;
        _activeKeyboard.SetText(text);
        return _activeKeyboard;
    }

    public ColorPicker ShowColorPicker(Color color)
    {
        Vector3 lookDirection = cameraTransform.forward;
        lookDirection.y = 0;
        ColorPicker colorPicker = CreateUI(colorPickerPrefab, cameraTransform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up)).GetComponent<ColorPicker>();
        colorPicker.CurrentColor = color;
        return colorPicker;
    }

    private GameObject CreateUI(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject go = Instantiate(prefab, position, rotation, parent);
        if (go.GetComponent<Canvas>() != null) go.GetComponent<Canvas>().worldCamera = cameraTransform.GetComponent<Camera>();
        return go;
    }
}
