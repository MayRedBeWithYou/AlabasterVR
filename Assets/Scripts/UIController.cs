// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="UIController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class UIController.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class UIController : MonoBehaviour
{
    private static UIController _instance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static UIController Instance => _instance;

    /// <summary>
    /// The main menu button handler
    /// </summary>
    [Header("Buttons")]
    public ButtonHandler MainMenuButtonHandler;
    /// <summary>
    /// The tool selection menu button handler
    /// </summary>
    public ButtonHandler ToolSelectionMenuButtonHandler;
    /// <summary>
    /// The layer selection menu button handler
    /// </summary>
    public ButtonHandler LayerSelectionMenuButtonHandler;
    /// <summary>
    /// The left touch handler
    /// </summary>
    public ButtonHandler LeftTouchHandler;

    /// <summary>
    /// The camera transform
    /// </summary>
    [Header("Menu parameters")]
    public Transform cameraTransform;
    /// <summary>
    /// The left hand menu transform
    /// </summary>
    public Transform LeftHandMenuTransform;

    /// <summary>
    /// The UI distance
    /// </summary>
    public float uiDistance;

    /// <summary>
    /// The main menu prefab
    /// </summary>
    [Header("Menu prefabs")]

    public GameObject MainMenuPrefab;
    /// <summary>
    /// The tool selection menu prefab
    /// </summary>
    public GameObject ToolSelectionMenuPrefab;
    /// <summary>
    /// The tabbed panel menu prefab
    /// </summary>
    public GameObject TabbedPanelMenuPrefab;
    /// <summary>
    /// The file explorer prefab
    /// </summary>
    public GameObject FileExplorerPrefab;
    /// <summary>
    /// The message box prefab
    /// </summary>
    public GameObject MessageBoxPrefab;
    /// <summary>
    /// The yes no cancel popup prefab
    /// </summary>
    public GameObject YesNoCancelPopupPrefab;
    /// <summary>
    /// The color picker prefab
    /// </summary>
    public GameObject colorPickerPrefab;
    /// <summary>
    /// The keyboard prefab
    /// </summary>
    public GameObject KeyboardPrefab;
    /// <summary>
    /// The picture canvas prefab
    /// </summary>
    public GameObject PictureCanvasPrefab;
    /// <summary>
    /// The left overlay
    /// </summary>
    public GameObject LeftOverlay;
    /// <summary>
    /// The layer settings prefab
    /// </summary>
    public GameObject LayerSettingsPrefab;
    /// <summary>
    /// The light settings prefab
    /// </summary>
    public GameObject LightSettingsPrefab;
    /// <summary>
    /// The pottery UI prefab
    /// </summary>
    public GameObject PotteryUIPrefab;

    private Keyboard _activeKeyboard = null;
    private MainMenu _activeMainMenu = null;
    private FileExplorer _activeFileExplorer = null;
    private GameObject _activeLeftHandMenu = null;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
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

        LeftOverlay.SetActive(false);
        LeftTouchHandler.OnButtonDown += (c) => LeftOverlay.SetActive(true);
        LeftTouchHandler.OnButtonUp += (c) => LeftOverlay.SetActive(false);
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
            _activeMainMenu.OpenButton.onClick.AddListener(() => ShowLoadModel());
            _activeMainMenu.SaveButton.onClick.AddListener(() => ShowSaveModel());
            _activeMainMenu.ImportButton.onClick.AddListener(() => ShowImportModel());
            _activeMainMenu.ExportButton.onClick.AddListener(() => ShowExportModel());
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
            _activeLeftHandMenu.transform.localPosition += new Vector3(0f, -0.08f, 0f);
        }
    }

    private void ShowLayerSelectionMenu(XRController controller)
    {
        if (_activeLeftHandMenu)
        {
            CloseLeftMenu();
        }
        else _activeLeftHandMenu = CreateUI(TabbedPanelMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);

    }

    /// <summary>
    /// Shows the tabbed panel menu.
    /// </summary>
    /// <returns>TabbedPanel.</returns>
    public TabbedPanel ShowTabbedPanelMenu()
    {
        if (_activeLeftHandMenu)
        {
            CloseLeftMenu();
        }
        _activeLeftHandMenu = CreateUI(TabbedPanelMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        return _activeLeftHandMenu.GetComponent<TabbedPanel>();
    }

    /// <summary>
    /// Closes the left menu.
    /// </summary>
    public void CloseLeftMenu()
    {
        if (_activeLeftHandMenu)
        {
            _activeLeftHandMenu.SetActive(false);
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }
    }

    /// <summary>
    /// Shows the keyboard.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>Keyboard.</returns>
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

    /// <summary>
    /// Shows the keyboard.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="text">The text.</param>
    /// <returns>Keyboard.</returns>
    public Keyboard ShowKeyboard(GameObject parent, string text)
    {
        if (_activeKeyboard != null)
        {
            _activeKeyboard.Close();
        }

        GameObject go = CreateUI(KeyboardPrefab, parent.transform.position, parent.transform.rotation);
        go.transform.localPosition -= Vector3.forward * Random.Range(0.001f, 0.01f);

        _activeKeyboard = go.GetComponent<Keyboard>();
        _activeKeyboard.OnClosing += () => _activeKeyboard = null;
        _activeKeyboard.SetText(text);
        return _activeKeyboard;
    }

    /// <summary>
    /// Shows the layer settings.
    /// </summary>
    /// <returns>LayerSettings.</returns>
    public LayerSettings ShowLayerSettings()
    {
        if (_activeLeftHandMenu != null)
        {
            _activeLeftHandMenu.SetActive(false);
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }

        _activeLeftHandMenu = CreateUI(LayerSettingsPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        return _activeLeftHandMenu.GetComponent<LayerSettings>();
    }

    /// <summary>
    /// Shows the light settings.
    /// </summary>
    /// <returns>LightSettings.</returns>
    public LightSettings ShowLightSettings()
    {
        if (_activeLeftHandMenu != null)
        {
            _activeLeftHandMenu.SetActive(false);
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }

        _activeLeftHandMenu = CreateUI(LightSettingsPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        return _activeLeftHandMenu.GetComponent<LightSettings>();
    }

    /// <summary>
    /// Shows the pottery UI.
    /// </summary>
    /// <returns>PotteryUI.</returns>
    public PotteryUI ShowPotteryUI()
    {
        if (_activeLeftHandMenu != null)
        {
            _activeLeftHandMenu.SetActive(false);
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
        }

        _activeLeftHandMenu = CreateUI(PotteryUIPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        _activeLeftHandMenu.transform.localPosition += new Vector3(0f, -0.08f, 0f);
        return _activeLeftHandMenu.GetComponent<PotteryUI>();
    }


    /// <summary>
    /// Shows the color picker.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>ColorPicker.</returns>
    public ColorPicker ShowColorPicker(Color color)
    {
        Vector3 lookDirection = cameraTransform.forward;
        lookDirection.y = 0;
        ColorPicker colorPicker = CreateUI(colorPickerPrefab, cameraTransform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up)).GetComponent<ColorPicker>();
        colorPicker.CurrentColor = color;
        return colorPicker;
    }

    /// <summary>
    /// Shows the message box.
    /// </summary>
    /// <param name="message">The message.</param>
    public void ShowMessageBox(string message)
    {
        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        Vector3 prefabPosition = Camera.main.transform.position + lookDirection.normalized * (uiDistance);
        GameObject go = CreateUI(MessageBoxPrefab, prefabPosition, Quaternion.LookRotation(lookDirection, Vector3.up));
        go.GetComponent<MessageBox>().Init(message);
    }

    /// <summary>
    /// Shows the message box.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="message">The message.</param>
    public void ShowMessageBox(GameObject parent, string message)
    {
        GameObject go = CreateUI(MessageBoxPrefab, parent.transform.position, parent.transform.rotation);
        go.transform.localPosition -= Vector3.forward * Random.Range(0.001f, 0.01f);
        go.GetComponent<MessageBox>().Init(message);
    }

    /// <summary>
    /// Shows the yes no popup.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>YesNoCancelPopup.</returns>
    public YesNoCancelPopup ShowYesNoPopup(string message)
    {
        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        Vector3 prefabPosition = Camera.main.transform.position + lookDirection.normalized * (uiDistance);
        GameObject go = CreateUI(YesNoCancelPopupPrefab, prefabPosition, Quaternion.LookRotation(lookDirection, Vector3.up));
        go.GetComponent<YesNoCancelPopup>().Init(message);
        return go.GetComponent<YesNoCancelPopup>();
    }

    /// <summary>
    /// Shows the yes no popup.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="message">The message.</param>
    /// <returns>YesNoCancelPopup.</returns>
    public YesNoCancelPopup ShowYesNoPopup(GameObject parent, string message)
    {
        GameObject go = CreateUI(YesNoCancelPopupPrefab, parent.transform.position, parent.transform.rotation);
        go.transform.localPosition -= Vector3.forward * Random.Range(0.001f, 0.01f);
        go.GetComponent<YesNoCancelPopup>().Init(message);
        return go.GetComponent<YesNoCancelPopup>();
    }

    /// <summary>
    /// Shows the export model.
    /// </summary>
    /// <returns>FileExplorer.</returns>
    public FileExplorer ShowExportModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.ExportModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }

    /// <summary>
    /// Shows the import model.
    /// </summary>
    /// <returns>FileExplorer.</returns>
    public FileExplorer ShowImportModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.ImportModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
    /// <summary>
    /// Shows the save model.
    /// </summary>
    /// <returns>FileExplorer.</returns>
    public FileExplorer ShowSaveModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.SaveModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
    /// <summary>
    /// Shows the load model.
    /// </summary>
    /// <returns>FileExplorer.</returns>
    public FileExplorer ShowLoadModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.LoadModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
    /// <summary>
    /// Shows the reference picture.
    /// </summary>
    /// <returns>FileExplorer.</returns>
    public FileExplorer ShowRefPicture()
    {
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.LoadImageReference(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
    /// <summary>
    /// Shows the picture canvas.
    /// </summary>
    /// <returns>PictureCanvas.</returns>
    public PictureCanvas ShowPictureCanvas()
    {
        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        Vector3 prefabPosition = Camera.main.transform.position + lookDirection.normalized * (uiDistance * 2);
        var canvas = CreateUI(PictureCanvasPrefab, prefabPosition, Quaternion.LookRotation(lookDirection, Vector3.up)).GetComponent<PictureCanvas>();
        return canvas;
    }

    private GameObject CreateUI(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject go = Instantiate(prefab, position, rotation, parent);
        if (go.GetComponent<Canvas>() != null) go.GetComponent<Canvas>().worldCamera = cameraTransform.GetComponent<Camera>();
        return go;
    }

    private FileExplorer PrepreparedFileExplorer()
    {
        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        Vector3 prefabPosition = Camera.main.transform.position + lookDirection.normalized * (uiDistance + 0.1f);
        var fileExplorer = CreateUI(FileExplorerPrefab, prefabPosition, Quaternion.LookRotation(lookDirection, Vector3.up));
        var script = fileExplorer.GetComponent<FileExplorer>();
        script.OnCancelled += () => script.Close();
        return script;
    }
}
