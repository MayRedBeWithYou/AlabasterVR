using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileExplorer : MonoBehaviour
{
    public enum FileMode
    {
        Open,
        Save
    }

    public FileMode mode;

    public string fileExtensions;

    [SerializeField]
    private Text headerText;

    [SerializeField]
    private GameObject entryHolder;

    [SerializeField]
    private Button acceptButton;

    [SerializeField]
    private Button cancelButton;

    [SerializeField]
    private GameObject itemPrefab;

    public delegate void SelectPathCallback(string path);
    public event SelectPathCallback OnAccepted;

    public delegate void CancelCallback();
    public event CancelCallback OnCancelled;

    public delegate void CloseCallback();
    public event CloseCallback OnClosing;

    public string SelectedPath;

    public void SetPath(string path)
    {

    }

    public void Start()
    {
        acceptButton.onClick.AddListener(() => OnAccepted?.Invoke(SelectedPath));
        cancelButton.onClick.AddListener(() => OnCancelled?.Invoke());
    }

    public void Close()
    {
        OnClosing?.Invoke();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
