// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="DirectoryButtonManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.UI;
using System.IO;
/// <summary>
/// Class DirectoryButtonManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class DirectoryButtonManager : MonoBehaviour
{
    /// <summary>
    /// The directory information
    /// </summary>
    public DirectoryInfo dirInfo;
    /// <summary>
    /// FileExplorer UI object.
    /// </summary>
    public FileExplorer mainScript;
    Button button;
    Text buttonText;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    /// <summary>
    /// Deactivates this instance.
    /// </summary>
    public void Deactivate()
    {
        button.enabled = false;
        buttonText.text = "";
        dirInfo = null;
    }

    /// <summary>
    /// Activates with specified directory information.
    /// </summary>
    /// <param name="directoryInfo">The directory information.</param>
    /// <param name="prefix">if set to <c>true</c>, adds the prefix.</param>
    public void Activate(DirectoryInfo directoryInfo, bool prefix)
    {
        button.enabled = true;
        if (directoryInfo != null)
        {
            if (prefix) buttonText.text = $"> {Wraper(directoryInfo.Name)}";
            else buttonText.text = Wraper(directoryInfo.Name);
        }
        else buttonText.text = "\\";
        dirInfo = directoryInfo;
    }

    /// <summary>
    /// Called when directory is clicked.
    /// </summary>
    /// 
    public void OnClick()
    {
        mainScript.ChangeDirectory(dirInfo);
    }

    string Wraper(string text)
    {
        if(text.Length>8)return text.Substring(0,6)+"..";
        else return text;
    }
}
