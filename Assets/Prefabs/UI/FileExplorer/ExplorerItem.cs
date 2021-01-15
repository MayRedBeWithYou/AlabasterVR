// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-04-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-04-2021
// ***********************************************************************
// <copyright file="ExplorerItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class ExplorerItem.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ExplorerItem : MonoBehaviour
{
    private FileType type;

    /// <summary>
    /// The filename
    /// </summary>
    public string filename;

    /// <summary>
    /// The explorer
    /// </summary>
    public FileExplorer explorer;

    /// <summary>
    /// The icon
    /// </summary>
    public Image icon;

    /// <summary>
    /// The name text
    /// </summary>
    public Text nameText;

    /// <summary>
    /// The picture icon
    /// </summary>
    [Header("Icons")]
    public Sprite pictureIcon;
    /// <summary>
    /// The model icon
    /// </summary>
    public Sprite modelIcon;
    /// <summary>
    /// The directory icon
    /// </summary>
    public Sprite directoryIcon;

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
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

    /// <summary>
    /// Gets or sets the filename.
    /// </summary>
    /// <value>The filename.</value>
    public string Filename
    {
        get => filename;
        set
        {
            filename = value;
            nameText.text = filename;
        }
    }

    /// <summary>
    /// Selects the item.
    /// </summary>
    public void SelectItem()
    {
        if (Type == FileType.Directory) explorer.ChangeDirectory(filename);
        else explorer.Filename = filename;
    }
}
public enum FileType
{
    /// <summary>
    /// The directory
    /// </summary>
    Directory,
    /// <summary>
    /// The model
    /// </summary>
    Model,
    /// <summary>
    /// The picture
    /// </summary>
    Picture
}
