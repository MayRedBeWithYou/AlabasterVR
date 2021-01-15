// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-08-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-08-2021
// ***********************************************************************
// <copyright file="PictureItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
/// <summary>
/// Class PictureItem.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class PictureItem : MonoBehaviour
{
    [SerializeField]
    Image hideImage;
    [SerializeField]
    Text picName;
    /// <summary>
    /// The canvas
    /// </summary>
    public PictureCanvas canvas;
    /// <summary>
    /// Gets or sets the name of the pic.
    /// </summary>
    /// <value>The name of the pic.</value>
    public string PicName { get { return picName.text; } set { picName.text = value; } }
    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        RefPictureManager.Instance.RemovePicture(canvas);
    }
    /// <summary>
    /// Hides this instance.
    /// </summary>
    public void Hide()
    {
        RefPictureManager.Instance.HidePicture(canvas);
    }
    /// <summary>
    /// Changes the name.
    /// </summary>
    public void ChangeName()
    {
        RefPictureManager.Instance.ChangeName(canvas);
    }
    /// <summary>
    /// Changes the visibility button.
    /// </summary>
    public void ChangeVisibilityButton()
    {
        if (canvas.visible) hideImage.color = Color.white;
        else hideImage.color = Color.black;
    }
}
