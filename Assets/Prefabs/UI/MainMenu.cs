// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="MainMenu.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class MainMenu.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class MainMenu : MonoBehaviour
{
    private YesNoCancelPopup popup;

    /// <summary>
    /// Creates new filebutton.
    /// </summary>
    public Button NewFileButton;
    /// <summary>
    /// The open button
    /// </summary>
    public Button OpenButton;
    /// <summary>
    /// The save button
    /// </summary>
    public Button SaveButton;

    /// <summary>
    /// The import button
    /// </summary>
    public Button ImportButton;
    /// <summary>
    /// The export button
    /// </summary>
    public Button ExportButton;

    /// <summary>
    /// The exit button
    /// </summary>
    public Button ExitButton;

    /// <summary>
    /// Creates new file.
    /// </summary>
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

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
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