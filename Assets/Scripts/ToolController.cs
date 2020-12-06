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

    public XRRig rig;

    [Header("Buttons")]
    public ButtonHandler MainMenuButtonHandler;
    public ButtonHandler ToolSelectionMenuButtonHandler;

    [Header("Menus")]

    public GameObject MainMenuPrefab;
    public float uiDistance = 0f;

    public GameObject ToolSelectionMenuPrefab;
    public Transform ToolSelectionMenuTransform;

    [Header("Keyboard")]
    public GameObject KeyboardPrefab;
    private Keyboard _activeKeyboard = null;

    public ButtonHandler KeyboardTest;

    private static ToolController _instance;
    public static ToolController Instance => _instance;

    private Tool _selectedTool;

    public List<Tool> ToolPrefabs = new List<Tool>();

    [HideInInspector]
    public List<Tool> Tools = new List<Tool>();

    private GameObject _activeMainMenu = null;
    private GameObject _activeToolSelectionMenu = null;

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
        foreach (Tool tool in ToolPrefabs)
        {
            Tools.Add(Instantiate(tool, transform));
        }
        SelectedTool = Tools[0];

        KeyboardTest.OnButtonDown += (controller) => ShowKeyboard();
    }

    private void ShowMainMenu(XRController controller)
    {
        if (_activeMainMenu)
        {
            CloseMainMenu();
        }
        else
        {
            Vector3 lookDirection = cameraTransform.forward;
            lookDirection.y = 0;
            _activeMainMenu = Instantiate(MainMenuPrefab, cameraTransform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up));
            MainMenuController mainMenu = _activeMainMenu.GetComponent<MainMenuController>();
            mainMenu.ExitButton.onClick.AddListener(CloseMainMenu);
        }
    }

    private void ShowToolSelectionMenu(XRController controller)
    {
        if (_activeToolSelectionMenu)
        {
            Destroy(_activeToolSelectionMenu);
            _activeToolSelectionMenu = null;
        }
        else
        {
            _activeToolSelectionMenu = Instantiate(ToolSelectionMenuPrefab, ToolSelectionMenuTransform.position, ToolSelectionMenuTransform.rotation, ToolSelectionMenuTransform);
        }
    }

    private void CloseMainMenu()
    {
        Destroy(_activeMainMenu);
        _activeMainMenu = null;
    }

    public Keyboard ShowKeyboard()
    {
        if(_activeKeyboard != null)
        {
            _activeKeyboard.Close();
        }

        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        GameObject go = Instantiate(KeyboardPrefab, Camera.main.transform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up));

        _activeKeyboard = go.GetComponent<Keyboard>();
        _activeKeyboard.OnCancelled += () => _activeKeyboard = null;
        return _activeKeyboard;
    }
}