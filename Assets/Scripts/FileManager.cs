using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{
    private static FileManager _instance;
    public static FileManager Instance { get { return _instance; } }

    private string path;

    void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
    }

    public FileExplorer SaveModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Save;
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              path = text;
              script.Close();
              if (!System.String.IsNullOrWhiteSpace(path))
              {
                  TranslateModelToObj(path);
              }
          };
        return script;
    }

    public FileExplorer LoadModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".obj" });
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              path = text;
              script.Close();
          };
        //todo:meshtosdf

        return script;
    }
    public FileExplorer LoadImageReference(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".jpg", ".png" });
        script.UpdateDirectory();
        return script;
    }

    private void TranslateModelToObj(string path)
    {
        string tempName = path;
        int counter = 1;
        bool nameChanged = false;
        if (File.Exists(tempName + ".obj"))
        {
            while (File.Exists($"{tempName}{counter}.obj")) counter++;
            nameChanged = true;
            tempName = tempName + counter.ToString();
        }
        tempName += ".obj";
        //todo: saving model

        if (!nameChanged) UIController.Instance.ShowMessageBox("Model zapisano jako " + Path.GetFileName(tempName));
        else UIController.Instance.ShowMessageBox($"Model o nazwie {Path.GetFileName(path)}.obj już istniał.\nModel zapisano jako {Path.GetFileName(tempName)}.");
    }
}
