// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-08-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="RefPictureManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class RefPictureManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class RefPictureManager : MonoBehaviour
{
    /// <summary>
    /// The picture canvas prefab
    /// </summary>
    public GameObject PictureCanvasPrefab;
    /// <summary>
    /// The pictures holder
    /// </summary>
    public GameObject PicturesHolder;

    private static RefPictureManager _instance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static RefPictureManager Instance { get { return _instance; } }

    /// <summary>
    /// Delegate PictureChanged
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public delegate void PictureChanged(PictureCanvas canvas);

    /// <summary>
    /// Occurs when [picture added].
    /// </summary>
    public static event PictureChanged PictureAdded;
    /// <summary>
    /// Occurs when [picture removed].
    /// </summary>
    public static event PictureChanged PictureRemoved;
    /// <summary>
    /// Occurs when [picture hidden].
    /// </summary>
    public static event PictureChanged PictureHidden;
    /// <summary>
    /// Occurs when [picture name updated].
    /// </summary>
    public static event PictureChanged PictureNameUpdated;

    /// <summary>
    /// The pictures
    /// </summary>
    public List<PictureCanvas> pictures = new List<PictureCanvas>();
    void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }
    /// <summary>
    /// Adds the reference picture.
    /// </summary>
    public void AddRefPicture()
    {
        var script = UIController.Instance.ShowRefPicture();
        script.OnAccepted += (text) =>
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            PictureCanvas canvas = UIController.Instance.ShowPictureCanvas();
            pictures.Add(canvas);
            canvas.SetPicture(text);
            PictureAdded?.Invoke(canvas);
            script.Close();
        };
        
    }
    /// <summary>
    /// Removes the picture.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public void RemovePicture(PictureCanvas canvas)
    {
        pictures.Remove(canvas);
        canvas.gameObject.SetActive(false);
        Destroy(canvas.gameObject);
        PictureRemoved?.Invoke(canvas);
    }
    /// <summary>
    /// Hides the picture.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public void HidePicture(PictureCanvas canvas)
    {
        canvas.visible = !canvas.visible;
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        PictureHidden?.Invoke(canvas);
    }
    /// <summary>
    /// Changes the name.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public void ChangeName(PictureCanvas canvas)
    {
        Keyboard keyboard = Keyboard.Show(canvas.PicName);
        keyboard.OnAccepted += (text) =>
        {
            UpdateName(canvas, text);
            keyboard.Close();
        };
        keyboard.OnCancelled += () => keyboard.Close();
    }
    /// <summary>
    /// Updates the name.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="name">The name.</param>
    public void UpdateName(PictureCanvas canvas, string name)
    {
        canvas.PicName = name;
        PictureNameUpdated?.Invoke(canvas);
    }

}
