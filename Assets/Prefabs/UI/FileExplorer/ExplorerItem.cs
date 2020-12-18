using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerItem : MonoBehaviour
{
    public enum ItemType
    {
        File,
        Directory
    }

    private ItemType type;

    public ItemType Type
    {
        get => type;
        set
        {
            type = value;
            image.sprite = type == ItemType.File ? fileIcon : directoryIcon;
        }
    }

    public Image image;

    public string path;

    [SerializeField]
    private Text text;

    private string itemName;

    public string ItemName
    {
        get => itemName;
        set
        {
            itemName = value;
            text.text = itemName;
        }
    }

    public Sprite fileIcon;

    public Sprite directoryIcon;

    public void SelectItem()
    {
        switch (type)
        {
            case ItemType.File:

                break;
            case ItemType.Directory:

                break;
        }
    }
}
