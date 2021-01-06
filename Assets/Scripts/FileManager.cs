using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FileManager
{
    public static FileExplorer SaveModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Save;
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              TranslateModelToObj(text);
              script.Close();
          };
        return script;
    }

    public static FileExplorer LoadModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".obj" });
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              script.Close();
          };
        //todo:meshtosdf

        return script;
    }
    
    public static FileExplorer LoadImageReference(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".jpg", ".png" });
        script.UpdateDirectory();
        return script;
    }

    private static void TranslateModelToObj(string path)
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
