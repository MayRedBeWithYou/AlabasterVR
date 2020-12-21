using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerItem : MonoBehaviour
{
    private FileType type;
    public string filename;
    public FileExplorer mainScript;
    void Awake()
    {

    }
    public FileType Type
    {
        get => type;
        set
        {
            type = value;
            var pic=gameObject.transform.GetChild(0).GetComponent<Image>();
            
            pic.sprite = directoryIcon;
            if(type==FileType.Picture) pic.sprite=pictureIcon;
            else if(type==FileType.Model) pic.sprite=modelIcon;
            
        }
    }

    public string Filename
    {
        get => filename;
        set
        {
            filename = value;
            gameObject.GetComponentInChildren<Text>().text=filename;
        }
    }
    
    public void SelectItem()
    {
        if(Type==FileType.Directory) mainScript.ChangeDirectory(filename);
        else mainScript.Filename=filename;
    }
    private Image image;

    public Sprite pictureIcon;
    public Sprite modelIcon;
    public Sprite directoryIcon;
}
public enum FileType
{
    Directory,
    Model,
    Picture
}
