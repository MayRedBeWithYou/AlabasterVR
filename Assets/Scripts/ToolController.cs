using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolController : MonoBehaviour
{
    public GameObject rightController;
    public GameObject leftController;

    public Transform cameraTransform;
    public Transform leftPointer;
    public Transform rightPointer;

    public ButtonHandler MainMenuButtonHandler;
    public ButtonHandler LeftPointerButtonHandler;
    public ButtonHandler RightPointerButtonHandler;

    public GameObject MainMenuPrefab;

    private static ToolController _instance;
    public static ToolController Instance => _instance;

    private GameObject _activeMainMenu = null;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        MainMenuButtonHandler.OnButtonDown += ShowMainMenu;

        LeftPointerButtonHandler.OnButtonDown += (controller) => ToggleLineVisual(controller, true);
        LeftPointerButtonHandler.OnButtonUp += (controller) => ToggleLineVisual(controller, false);

        RightPointerButtonHandler.OnButtonDown += (controller) => ToggleLineVisual(controller, true);
        RightPointerButtonHandler.OnButtonUp += (controller) => ToggleLineVisual(controller, false);
    }

    private void ToggleLineVisual(XRController controller, bool value)
    {
        controller.GetComponentInParent<XRRayInteractor>().enabled = value;
        controller.GetComponentInParent<XRInteractorLineVisual>().enabled = value;
    }

    private void ShowMainMenu(XRController controller)
    {
        if (_activeMainMenu)
        {
            CloseMainMenu();
        }
        else
        {
            _activeMainMenu = Instantiate(MainMenuPrefab, leftPointer.position, Quaternion.LookRotation(leftController.transform.position - cameraTransform.position, Vector3.up));
            MainMenuController mainMenu = _activeMainMenu.GetComponent<MainMenuController>();
            mainMenu.ExitButton.onClick.AddListener(CloseMainMenu);
        }
    }

    private void CloseMainMenu()
    {
        Destroy(_activeMainMenu);
        _activeMainMenu = null;
    }
}
