using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum FileExplorerMode
{
    Open,
    Save
}
public class FileExplorer : MonoBehaviour
{
    private char[] forbidden = { '#', '%', '&', '{', '}', '\\', '<', '>', '*', '?', '/', ' ', '$', '!', '\'', '"', ':', '@', '+', '`', '|', '=' };

    [Header("Parameters")]
    public FileExplorerMode mode;
    public string SelectedPath;
    private string filename = "";
    private DirectoryInfo currentDirectory;
    public string[] fileExtensions;

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject contentHolder;
    [SerializeField]
    private InputField inputText;
    [SerializeField]
    private Text placeholderText;

    [Header("Buttons")]
    [SerializeField]
    private Button acceptButton;

    [SerializeField]
    private Button cancelButton;

    public Color validColor;
    public Color invalidColor;

    [Header("Path buttons scripts")]
    [SerializeField]
    private DirectoryButtonManager grandparentButtonScript;
    [SerializeField]
    private DirectoryButtonManager parentButtonScript;
    [SerializeField]
    private DirectoryButtonManager currentButtonScript;
    private List<Transform> items;

    private string resultPath
    {
        get
        {
            return currentDirectory.FullName + '/' + Filename;
        }
    }

    public string Filename
    {
        get
        {
            return filename;
        }
        set
        {
            filename = value;
            inputText.text = value;
            if (value == "")
            {
                if (mode == FileExplorerMode.Open)
                {
                    placeholderText.text = "Choose a file from list";
                    inputText.interactable = false;
                }
                else placeholderText.text = "Enter file name";
            }
            inputText.ForceLabelUpdate();
        }
    }

    public delegate void SelectPathCallback(string SelectedPath);
    public event SelectPathCallback OnAccepted;

    public delegate void CancelCallback();
    public event CancelCallback OnCancelled;

    void Awake()
    {
        currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        items = new List<Transform>();
        acceptButton.onClick.AddListener(() => OnAccepted?.Invoke(resultPath));
        cancelButton.onClick.AddListener(() => OnCancelled?.Invoke());
    }

    void Start()
    {
        Filename = "";
    }

    public void SetExtensionsArray(string[] arr)
    {
        fileExtensions = arr;
    }

    public void Close()
    {
        ClearItems();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void UpdateDirectory()
    {
        ClearItems();

        if (mode == FileExplorerMode.Open)
        {
            IEnumerable<FileInfo> files = currentDirectory.EnumerateFiles();
            IEnumerable<FileInfo> preselectedFiles = files.Where(f => fileExtensions.Contains(f.Extension));

            FileType type = FileType.Picture;
            if (fileExtensions[0] == ".obj") type = FileType.Model;

            foreach (var f in preselectedFiles)
            {
                AddExplorerItem(f.Name, type);
            }
        }
        var dirs = Directory.EnumerateDirectories(currentDirectory.FullName);
        foreach (var f in dirs)
        {
            AddExplorerItem(Path.GetFileName(f), FileType.Directory);
        }
        UpdateParentButtons();
        if (!acceptButton.enabled)
        {
            acceptButton.enabled = true;
            acceptButton.GetComponent<Image>().color = validColor;
        }

    }
    public void UpdateDirectoryRoot()
    {
        ClearItems();
        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (var f in drives)
        {
            if (f.DriveType == DriveType.Fixed || f.DriveType == DriveType.Network || f.DriveType == DriveType.Removable) AddExplorerItem(f.Name, FileType.Directory);
        }
        grandparentButtonScript.Deactivate();
        parentButtonScript.Deactivate();
        currentButtonScript.Deactivate();

        acceptButton.enabled = false;
        acceptButton.GetComponent<Image>().color = invalidColor;
    }

    private void AddExplorerItem(string filename, FileType type)
    {
        GameObject go = Instantiate(itemPrefab, contentHolder.transform);
        items.Add(go.transform);
        var script = go.GetComponent<ExplorerItem>();
        script.Filename = filename;
        script.Type = type;
        script.explorer = this;

    }
    private void UpdateParentButtons()
    {
        DirectoryInfo parent = currentDirectory.Parent;

        if (parent == null)
        {
            grandparentButtonScript.Activate(null, false);
            parentButtonScript.Activate(currentDirectory, false);
            currentButtonScript.Deactivate();
        }
        else
        {
            DirectoryInfo grandparent = parent.Parent;

            if (grandparent == null)
            {
                grandparentButtonScript.Activate(null, false);
                parentButtonScript.Activate(parent, false);
                currentButtonScript.Activate(currentDirectory, true);
            }
            else
            {
                grandparentButtonScript.Activate(grandparent, false);
                parentButtonScript.Activate(parent, true);
                currentButtonScript.Activate(currentDirectory, true);
            }
        }
    }
    public void ChangeDirectory(string name)
    {
        if (currentDirectory != null)
        {
            currentDirectory = new DirectoryInfo(currentDirectory.FullName + '/' + name);
            UpdateDirectory();
        }
        else
        {
            currentDirectory = new DirectoryInfo(name);
            UpdateDirectory();
        }
    }
    public void ChangeDirectory(DirectoryInfo directoryInfo)
    {
        if (directoryInfo != null)
        {
            currentDirectory = directoryInfo;
            UpdateDirectory();
        }
        else
        {
            currentDirectory = null;
            UpdateDirectoryRoot();
        }
    }

    private void ClearItems()
    {
        while (items.Count > 0)
        {
            var go = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            go.gameObject.SetActive(false);
            Destroy(go.gameObject);
        }
    }

    public void ShowKeyboard()
    {
        if (mode == FileExplorerMode.Save)
        {
            Keyboard keyboard = Keyboard.Show(gameObject, Filename);
            keyboard.OnAccepted += (text) =>
            {
                for (int i = 0; i < forbidden.Length; i++)
                {
                    if (text.Contains(forbidden[i]))
                    {
                        UIController.Instance.ShowMessageBox(keyboard.gameObject, "Invalid character appeared in file name:\n" + forbidden[i]);
                        return;
                    }
                }
                Filename = text;
                keyboard.Close();
            };
            keyboard.OnCancelled += () => keyboard.Close();
        }
    }
}
