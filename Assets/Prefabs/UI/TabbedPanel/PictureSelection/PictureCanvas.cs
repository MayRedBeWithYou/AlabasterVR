// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="PictureCanvas.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Class PictureCanvas.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="IResizable" />
/// Implements the <see cref="IMovable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="IResizable" />
/// <seealso cref="IMovable" />
public class PictureCanvas : MonoBehaviour, IResizable, IMovable
{
    [SerializeField]
    private Text picName;
    [SerializeField]
    private Image image;
    private string path;
    /// <summary>
    /// Gets or sets the name of the pic.
    /// </summary>
    /// <value>The name of the pic.</value>
    [HideInInspector]
    public string PicName { get { return picName.text; } set { picName.text = value; } }
    /// <summary>
    /// The visible
    /// </summary>
    public bool visible;

    private FileExplorer explorer;

    /// <summary>
    /// Updates the image.
    /// </summary>
    public void UpdateImage()
    {
        if (explorer != null) explorer.Close();
        explorer = UIController.Instance.ShowRefPicture();
        explorer.OnAccepted += (text) =>
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            SetPicture(text);
            explorer.Close();
        };
    }
    /// <summary>
    /// Sets the picture.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetPicture(string text)
    {
        path = text;
        PicName = Path.GetFileNameWithoutExtension(text);
        image.sprite = LoadNewSprite(text);
    }
    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        if (explorer != null) explorer.Close();
        RefPictureManager.Instance.RemovePicture(this);
    }
    /// <summary>
    /// Hides this instance.
    /// </summary>
    public void Hide()
    {
        RefPictureManager.Instance.HidePicture(this);
    }
    /// <summary>
    /// Changes the name.
    /// </summary>
    public void ChangeName()
    {
        RefPictureManager.Instance.ChangeName(this);
    }
    private Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        //https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/        

        Texture2D texture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

        return NewSprite;
    }

    private Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(1, 1);
            if (Tex2D.LoadImage(FileData)) return Tex2D;
            else return null;
        }
        return null;
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    /// <param name="scale">The scale.</param>
    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="pos">The position.</param>
    public void SetPosition(Vector3 pos)
    {

    }

    /// <summary>
    /// Sets the rotation.
    /// </summary>
    /// <param name="rot">The rot.</param>
    public void SetRotation(Quaternion rot)
    {

    }
}
