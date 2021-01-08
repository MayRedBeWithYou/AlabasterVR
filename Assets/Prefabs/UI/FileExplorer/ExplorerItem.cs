using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerItem : MonoBehaviour
{
    private FileType type;

    public string filename;

    public FileExplorer explorer;

    public Image icon;

    public Text nameText;

    [Header("Icons")]
    public Sprite pictureIcon;
    public Sprite modelIcon;
    public Sprite directoryIcon;

    public FileType Type
    {
        get => type;
        set
        {
            type = value;
            icon.sprite = directoryIcon;
            if (type == FileType.Picture) icon.sprite = pictureIcon;
            else if (type == FileType.Model) icon.sprite = modelIcon;
        }
    }

    public string Filename
    {
        get => filename;
        set
        {
            filename = value;
            nameText.text = filename;
        }
    }

    public void SelectItem()
    {
        if (Type == FileType.Directory) explorer.ChangeDirectory(filename);
        else explorer.Filename = filename;
    }
}
public enum FileType
{
    Directory,
    Model,
    Picture
}
