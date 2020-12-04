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

    [Header("Menus")]

    public GameObject MainMenuPrefab;
    public float MainMenuDistance = 0f;

    public GameObject ToolSelectionMenuPrefab;
    public Transform ToolSelectionMenuTransform;

    [Header("Keyboard")]
    public ButtonHandler KeyboardHandler;
    public GameObject KeyboardPrefab;
    private GameObject _activeKeyboard = null;



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

        KeyboardHandler.OnButtonDown+=ShowKeyboard;
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

    private void ShowKeyboard(XRController controller)
    {
        Debug.Log("ShowKeyboard");
        if (_activeKeyboard)
        {
            Destroy(_activeKeyboard);
            _activeKeyboard = null;
        }
        else
        {
            Vector3 lookDirection = leftController.transform.position - cameraTransform.position;
            lookDirection.y = 0;
            _activeKeyboard = Instantiate(KeyboardPrefab, leftPointer.position + lookDirection.normalized * MainMenuDistance, Quaternion.LookRotation(lookDirection, Vector3.up));
            var KeyboardPanelTransform=_activeKeyboard.transform.GetChild(0).GetChild(0).GetChild(1);
            UnityEngine.UI.Button bCancel=KeyboardPanelTransform.GetChild(4).GetChild(5).gameObject.GetComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.Button bConfirm=KeyboardPanelTransform.GetChild(2).GetChild(12).gameObject.GetComponent<UnityEngine.UI.Button>();
            
            bCancel.onClick.AddListener(CloseKeyboard);
            bConfirm.onClick.AddListener(CloseKeyboard);
        }
    }
    private void CloseKeyboard()
    {
        Destroy(_activeKeyboard);
        _activeKeyboard = null;
    }    
}