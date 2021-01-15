// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-08-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-08-2021
// ***********************************************************************
// <copyright file="RefPicturesSelection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class RefPicturesSelection.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class RefPicturesSelection : MonoBehaviour
{
    private static RefPicturesSelection _instance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static RefPicturesSelection Instance { get { return _instance; } }
    /// <summary>
    /// The content holder
    /// </summary>
    public GameObject contentHolder;
    /// <summary>
    /// The reference picture item prefab
    /// </summary>
    public GameObject refPictureItemPrefab;
    /// <summary>
    /// The pictures
    /// </summary>
    public Dictionary<PictureCanvas, PictureItem> pictures;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        pictures = new Dictionary<PictureCanvas, PictureItem>();

        foreach (PictureCanvas canvas in RefPictureManager.Instance.pictures)
        {
            PictureAdded(canvas);
        }
        RefPictureManager.PictureAdded+=PictureAdded;
        RefPictureManager.PictureRemoved+=PictureRemoved;
        RefPictureManager.PictureHidden+=PicturesHidden;
        RefPictureManager.PictureNameUpdated+=PictureNameUpdated;
    }

    private void PictureRemoved(PictureCanvas canvas)
    {
        PictureItem item = pictures[canvas];
        pictures.Remove(canvas);
        item.gameObject.SetActive(false);
        Destroy(item.gameObject);
    }

    private void PictureAdded(PictureCanvas canvas)
    {
        PictureItem item = Instantiate(refPictureItemPrefab, contentHolder.transform).GetComponent<PictureItem>();
        pictures.Add(canvas, item);
        item.canvas=canvas;
        item.ChangeVisibilityButton();
        PictureNameUpdated(canvas);
    }
    /// <summary>
    /// Pictureses the hidden.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public void PicturesHidden(PictureCanvas canvas)
    {
        pictures[canvas].ChangeVisibilityButton();   
    }

    private void PictureNameUpdated(PictureCanvas canvas)
    {
        pictures[canvas].PicName=canvas.PicName;
    }
    /// <summary>
    /// Called when [destroy].
    /// </summary>
    public void OnDestroy()
    {
        RefPictureManager.PictureAdded-=PictureAdded;
        RefPictureManager.PictureRemoved-=PictureRemoved;
        RefPictureManager.PictureHidden-=PicturesHidden;
        RefPictureManager.PictureNameUpdated-=PictureNameUpdated;
    }
}
