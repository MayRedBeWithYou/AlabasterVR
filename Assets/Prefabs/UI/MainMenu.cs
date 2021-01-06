using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MainMenu : MonoBehaviour
{
    private YesNoCancelPopup popup;

    public Button NewFileButton;

    public Button SaveButton;

    public Button ImportButton;

    public Button OptionsButton;

    public Button ExitButton;

    public void NewFile()
    {
        if (popup != null) popup.Close();
        popup = UIController.Instance.ShowYesNoPopup(gameObject, "Do you wish to save your work?");
        popup.OnAccept += () =>
        {
            FileExplorer explorer = UIController.Instance.ShowSaveModel();
            explorer.OnAccepted += (file) => LayerManager.Instance.ResetLayers();
        };
        popup.OnCancel += () => popup.Close();
        popup.OnDecline += () => LayerManager.Instance.ResetLayers();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void QuitApp()
    {
        if (popup != null) popup.Close();
        popup = UIController.Instance.ShowYesNoPopup(gameObject, "Do you wish to save your work before leaving?");
        popup.OnAccept += () =>
        {
            FileExplorer explorer = UIController.Instance.ShowSaveModel();
            explorer.OnAccepted += (file) => Application.Quit();
        };
        popup.OnCancel += () => popup.Close();
        popup.OnDecline += () => Application.Quit();
    }

}