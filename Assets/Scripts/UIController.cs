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
    public ButtonHandler LeftTouchHandler;
    public ButtonHandler SnapshotButton;

    [Header("Menu parameters")]
    public Transform cameraTransform;
    public Transform LeftHandMenuTransform;

    public float uiDistance;

    [Header("Menu prefabs")]

    public GameObject MainMenuPrefab;
    public GameObject ToolSelectionMenuPrefab;
    public GameObject TabbedPanelMenuPrefab;
    public GameObject FileExplorerPrefab;
    public GameObject MessageBoxPrefab;
    public GameObject YesNoCancelPopupPrefab;
    public GameObject colorPickerPrefab;
    public GameObject KeyboardPrefab;
    public GameObject PictureCanvasPrefab;
    public GameObject LeftOverlay;
    public GameObject LayerSettingsPrefab;
    public GameObject LightSettingsPrefab;
    public GameObject PotteryUIPrefab;

    private Keyboard _activeKeyboard = null;
    private MainMenu _activeMainMenu = null;
    private FileExplorer _activeFileExplorer = null;
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

        LeftOverlay.SetActive(false);
        LeftTouchHandler.OnButtonDown += (c) => LeftOverlay.SetActive(true);
        LeftTouchHandler.OnButtonUp += (c) => LeftOverlay.SetActive(false);
        SnapshotButton.OnButtonDown += (c) => MakeSnapshot();
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

    public TabbedPanel ShowTabbedPanelMenu()
    {
        if (_activeLeftHandMenu)
        {
            CloseLeftMenu();
        }
        _activeLeftHandMenu = CreateUI(TabbedPanelMenuPrefab, LeftHandMenuTransform.position, LeftHandMenuTransform.rotation, LeftHandMenuTransform);
        return _activeLeftHandMenu.GetComponent<TabbedPanel>();
    }

    public void CloseLeftMenu()
    {
        if (_activeLeftHandMenu)
        {
            _activeLeftHandMenu.SetActive(false);
            Destroy(_activeLeftHandMenu);
            _activeLeftHandMenu = null;
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


    public ColorPicker ShowColorPicker(Color color)
    {
        Vector3 lookDirection = cameraTransform.forward;
        lookDirection.y = 0;
        ColorPicker colorPicker = CreateUI(colorPickerPrefab, cameraTransform.position + lookDirection.normalized * uiDistance, Quaternion.LookRotation(lookDirection, Vector3.up)).GetComponent<ColorPicker>();
        colorPicker.CurrentColor = color;
        return colorPicker;
    }

    public void ShowMessageBox(string message)
    {
        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        Vector3 prefabPosition = Camera.main.transform.position + lookDirection.normalized * (uiDistance);
        GameObject go = CreateUI(MessageBoxPrefab, prefabPosition, Quaternion.LookRotation(lookDirection, Vector3.up));
        go.GetComponent<MessageBox>().Init(message);
    }

    public void ShowMessageBox(GameObject parent, string message)
    {
        GameObject go = CreateUI(MessageBoxPrefab, parent.transform.position, parent.transform.rotation);
        go.transform.localPosition -= Vector3.forward * Random.Range(0.001f, 0.01f);
        go.GetComponent<MessageBox>().Init(message);
    }

    public YesNoCancelPopup ShowYesNoPopup(string message)
    {
        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        Vector3 prefabPosition = Camera.main.transform.position + lookDirection.normalized * (uiDistance);
        GameObject go = CreateUI(YesNoCancelPopupPrefab, prefabPosition, Quaternion.LookRotation(lookDirection, Vector3.up));
        go.GetComponent<YesNoCancelPopup>().Init(message);
        return go.GetComponent<YesNoCancelPopup>();
    }

    public YesNoCancelPopup ShowYesNoPopup(GameObject parent, string message)
    {
        GameObject go = CreateUI(YesNoCancelPopupPrefab, parent.transform.position, parent.transform.rotation);
        go.transform.localPosition -= Vector3.forward * Random.Range(0.001f, 0.01f);
        go.GetComponent<YesNoCancelPopup>().Init(message);
        return go.GetComponent<YesNoCancelPopup>();
    }

    public FileExplorer ShowExportModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.ExportModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }

    public FileExplorer ShowImportModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.ImportModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
    public FileExplorer ShowSaveModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.SaveModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
    public FileExplorer ShowLoadModel()
    {
        if (_activeMainMenu != null) _activeMainMenu.Close();
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.LoadModel(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
    public FileExplorer ShowRefPicture()
    {
        if (_activeFileExplorer != null) _activeFileExplorer.Close();
        _activeFileExplorer = FileManager.LoadImageReference(PrepreparedFileExplorer());
        return _activeFileExplorer;
    }
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

    private void MakeSnapshot()
    {
        string name = "AlabasterSnapshot";
        int counter = 1;
        if (System.IO.File.Exists(name + ".png"))
        {
            while (System.IO.File.Exists($"{name}{counter}.png")) counter++;
            name += counter.ToString();
        }
        name += ".png";

        int captureWidth = 1920;
        int captureHeight = 1080;
        Rect rect = new Rect(0, 0, captureWidth, captureHeight);
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        Camera camera = cameraTransform.gameObject.GetComponent<Camera>();
        camera.targetTexture = renderTexture;
        camera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        byte[] fileData = null;
        fileData = screenShot.EncodeToPNG();
        var f = System.IO.File.Create(name);
        f.Write(fileData, 0, fileData.Length);
        ShowMessageBox("Snapshot saved as\n" + name);
    }

}
