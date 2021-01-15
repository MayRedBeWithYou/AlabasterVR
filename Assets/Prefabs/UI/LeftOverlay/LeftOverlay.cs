// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-07-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-07-2021
// ***********************************************************************
// <copyright file="LeftOverlay.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class LeftOverlay.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LeftOverlay : MonoBehaviour
{
    /// <summary>
    /// The undo image
    /// </summary>
    public Image undoImage;
    /// <summary>
    /// The redo image
    /// </summary>
    public Image redoImage;

    /// <summary>
    /// The valid color
    /// </summary>
    public Color validColor;
    /// <summary>
    /// The invalid color
    /// </summary>
    public Color invalidColor;

    void Start()
    {
        OperationManager.Instance.OnStacksChanged += RefreshIcons;

        RefreshIcons();
    }

    void OnEnable()
    {
        RefreshIcons();
    }

    void RefreshIcons()
    {
        if (OperationManager.Instance.undoOperations.Count > 0) undoImage.color = validColor;
        else undoImage.color = invalidColor;

        if (OperationManager.Instance.redoOperations.Count > 0) redoImage.color = validColor;
        else redoImage.color = invalidColor;
    }
}
