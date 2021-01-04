using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class DirectoryButtonManager : MonoBehaviour
{
    public DirectoryInfo dirInfo;
    public FileExplorer mainScript;
    Button button;
    Text buttonText;

    public void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    public void Deactivate()
    {
        button.enabled = false;
        buttonText.text = "";
        dirInfo = null;
    }
    public void Activate(DirectoryInfo directoryInfo, bool prefix)
    {
        button.enabled = true;
        if (directoryInfo != null)
        {
            if (prefix) buttonText.text = $"> {directoryInfo.Name}";
            else buttonText.text = directoryInfo.Name;
        }
        else buttonText.text = "\\";
        dirInfo = directoryInfo;
    }
    public void OnClick()
    {
        mainScript.ChangeDirectory(dirInfo);
    }
}
