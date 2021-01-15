// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-07-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-07-2021
// ***********************************************************************
// <copyright file="YesNoCancelPopup.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class YesNoCancelPopup.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class YesNoCancelPopup : MonoBehaviour
{
    /// <summary>
    /// The text
    /// </summary>
    public Text text;

    /// <summary>
    /// Delegate OnState
    /// </summary>
    public delegate void OnState();

    /// <summary>
    /// Occurs when [on accept].
    /// </summary>
    public event OnState OnAccept;

    /// <summary>
    /// Occurs when [on decline].
    /// </summary>
    public event OnState OnDecline;

    /// <summary>
    /// Occurs when [on cancel].
    /// </summary>
    public event OnState OnCancel;

    /// <summary>
    /// Initializes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Init(string message)
    {
        text.text = message;
    }

    /// <summary>
    /// Accepts this instance.
    /// </summary>
    public void Accept()
    {
        OnAccept?.Invoke();
        Close();
    }

    /// <summary>
    /// Declines this instance.
    /// </summary>
    public void Decline()
    {
        OnDecline?.Invoke();
        Close();
    }

    /// <summary>
    /// Cancels this instance.
    /// </summary>
    public void Cancel()
    {
        OnCancel?.Invoke();
        Close();
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
