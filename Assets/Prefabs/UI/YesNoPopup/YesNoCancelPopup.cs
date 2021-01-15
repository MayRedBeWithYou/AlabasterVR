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
    /// Occurs when user accepts.
    /// </summary>
    public event OnState OnAccept;

    /// <summary>
    /// Occurs when user declines.
    /// </summary>
    public event OnState OnDecline;

    /// <summary>
    /// Occurs when user cancels.
    /// </summary>
    public event OnState OnCancel;

    /// <summary>
    /// Initializes with specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Init(string message)
    {
        text.text = message;
    }

    /// <summary>
    /// Used when user accepts.
    /// </summary>
    public void Accept()
    {
        OnAccept?.Invoke();
        Close();
    }

    /// <summary>
    /// Used when user declines.
    /// </summary>
    public void Decline()
    {
        OnDecline?.Invoke();
        Close();
    }

    /// <summary>
    /// Used when user cancels.
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
