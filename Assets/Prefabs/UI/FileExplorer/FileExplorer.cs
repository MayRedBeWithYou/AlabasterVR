// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="FileExplorer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum FileExplorerMode
{
    /// <summary>
    /// The open
    /// </summary>
    Open,
    /// <summary>
    /// The save
    /// </summary>
    Save
}
/// <summary>
/// Class FileExplorer.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class FileExplorer : MonoBehaviour
{
    private char[] forbidden = { '#', '%', '&', '{', '}', '\\', '<', '>', '*', '?', '/', ' ', '$', '!', '\'', '"', ':', '@', '+', '`', '|', '=' };

    /// <summary>
    /// The mode
    /// </summary>
    [Header("Parameters")]
    public FileExplorerMode mode;
    /// <summary>
    /// The selected path
    /// </summary>
    public string SelectedPath;
    private string filename = "";
    private DirectoryInfo currentDirectory;
    /// <summary>
    /// The file extensions
    /// </summary>
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

    /// <summary>
    /// The valid color
    /// </summary>
    public Color validColor;
    /// <summary>
    /// The invalid color
    /// </summary>
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
            if(Filename==null || Filename=="")return "";
            return currentDirectory.FullName + '/' + Filename;
        }
    }

    /// <summary>
    /// Gets or sets the filename.
    /// </summary>
    /// <value>The filename.</value>
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

    /// <summary>
    /// Delegate SelectPathCallback
    /// </summary>
    /// <param name="SelectedPath">The selected path.</param>
    public delegate void SelectPathCallback(string SelectedPath);
    /// <summary>
    /// Occurs when [on accepted].
    /// </summary>
    public event SelectPathCallback OnAccepted;

    /// <summary>
    /// Delegate CancelCallback
    /// </summary>
    public delegate void CancelCallback();
    /// <summary>
    /// Occurs when [on cancelled].
    /// </summary>
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

    /// <summary>
    /// Sets the extensions array.
    /// </summary>
    /// <param name="arr">The arr.</param>
    public void SetExtensionsArray(string[] arr)
    {
        fileExtensions = arr;
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        ClearItems();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Updates the directory.
    /// </summary>
    public void UpdateDirectory()
    {
        ClearItems();

        if (mode == FileExplorerMode.Open)
        {
            IEnumerable<FileInfo> files = currentDirectory.EnumerateFiles();
            IEnumerable<FileInfo> preselectedFiles = files.Where(f => fileExtensions.Contains(f.Extension));

            FileType type = FileType.Picture;
            if (fileExtensions[0] == ".obj" || fileExtensions[0] == ".abs") type = FileType.Model;

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
    /// <summary>
    /// Updates the directory root.
    /// </summary>
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
    /// <summary>
    /// Changes the directory.
    /// </summary>
    /// <param name="name">The name.</param>
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
    /// <summary>
    /// Changes the directory.
    /// </summary>
    /// <param name="directoryInfo">The directory information.</param>
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

    /// <summary>
    /// Shows the keyboard.
    /// </summary>
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
