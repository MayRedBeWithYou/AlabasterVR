using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public float MainMenuDistance = 0f;

    public Transform LeftHandMenuTransform;

    public GameObject ToolSelectionMenuPrefab;
    public GameObject LayerSelectionMenuPrefab;

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
            CloseMainMenu();
        }
        else
        {
            Vector3 lookDirection = leftController.transform.position - cameraTransform.position;
            lookDirection.y = 0;
            _activeMainMenu = Instantiate(MainMenuPrefab, leftPointer.position + lookDirection.normalized * MainMenuDistance, Quaternion.LookRotation(lookDirection, Vector3.up));
            MainMenuController mainMenu = _activeMainMenu.GetComponent<MainMenuController>();
            mainMenu.ExitButton.onClick.AddListener(CloseMainMenu);
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
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }
        else
        {
            _activeLeftHandMenu = Instantiate(LayerSelectionMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        }
    }

    private void CloseMainMenu()
    {
        Destroy(_activeMainMenu);
        _activeMainMenu = null;
    }
}