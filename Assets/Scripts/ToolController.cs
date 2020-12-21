using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolController : MonoBehaviour
{
    [Header("Controller references")]
    public GameObject rightController;
    public GameObject leftController;

    public Transform cameraTransform;

    public Transform leftPointer;
    public Transform rightPointer;

    [Header("Buttons")]
    public ButtonHandler MainMenuButtonHandler;
    public ButtonHandler ToolSelectionMenuButtonHandler;
    public ButtonHandler LayerSelectionMenuButtonHandler;

    [Header("Menus")]

    public GameObject MainMenuPrefab;
    public float uiDistance;

    public Transform LeftHandMenuTransform;

    public GameObject ToolSelectionMenuPrefab;
    public GameObject LayerSelectionMenuPrefab;

    [Header("Keyboard")]
    public GameObject KeyboardPrefab;
    private Keyboard _activeKeyboard = null;

    private static ToolController _instance;
    public static ToolController Instance => _instance;

    private Tool _selectedTool;

    public List<Tool> ToolPrefabs = new List<Tool>();

    [HideInInspector]
    public List<Tool> Tools = new List<Tool>();

    private GameObject _activeMainMenu = null;
    
    private GameObject _activeLeftHandMenu = null;

    public delegate void ToolChanged(Tool tool);
    public static event ToolChanged SelectedToolChanged;

    public Tool SelectedTool
    {
        get => _selectedTool;
        set
        {
            foreach (Tool tool in Tools)
            {
                if (tool == value) tool.Enable();
                else tool.Disable();
            }
            _selectedTool = value;
            SelectedToolChanged?.Invoke(value);
            Debug.Log($"Selected tool changed to {value.name}");
        }
    }

    public void ToggleSelectedTool(bool value)
    {
        if (value) SelectedTool.Enable();
        else SelectedTool.Disable();
    }

    private void Awake()
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

        foreach (Tool tool in ToolPrefabs)
        {
            Tools.Add(Instantiate(tool, transform));
        }
        SelectedTool = Tools[0];
    }

    private void ShowMainMenu(XRController controller)
    {
        if (_activeMainMenu)
        {
            FileManager.Util.CloseFileExplorer();
            CloseMainMenu();
        }
        else
        {
            Vector3 lookDirection = cameraTransform.forward;
            lookDirection.y = 0;
            _activeMainMenu = Instantiate(MainMenuPrefab, cameraTransform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up));
            MainMenuController mainMenu = _activeMainMenu.GetComponent<MainMenuController>();
            mainMenu.ExitButton.onClick.AddListener(CloseMainMenu);
            mainMenu.SaveButton.onClick.AddListener(ShowSaveModel);
            mainMenu.ImportButton.onClick.AddListener(ShowLoadModel);
        }
    }

    private void ShowToolSelectionMenu(XRController controller)
    {
        if (_activeLeftHandMenu)
        {
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }
        else
        {
            _activeLeftHandMenu = Instantiate(ToolSelectionMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
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
            _activeLeftHandMenu = Instantiate(LayerSelectionMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        }
    }
    private void ShowSaveModel()
    {
        CloseMainMenu();
        _activeMainMenu=FileManager.Util.SaveModel();
    }
    private void ShowLoadModel()
    {
        CloseMainMenu();
        _activeMainMenu= FileManager.Util.LoadModel();
    }

    private void CloseMainMenu()
    {
        FileManager.Util.CloseFileExplorer();
        _activeMainMenu.SetActive(false);
        Destroy(_activeMainMenu);
        _activeMainMenu = null;
    }
    public Keyboard ShowKeyboard(string text)
    {
        if(_activeKeyboard != null)
        {
            _activeKeyboard.Close();
        }

        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        GameObject go = Instantiate(KeyboardPrefab, Camera.main.transform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up));

        _activeKeyboard = go.GetComponent<Keyboard>();
        _activeKeyboard.OnClosing += () => _activeKeyboard = null;
        _activeKeyboard.SetText(text);
        return _activeKeyboard;
    }
}