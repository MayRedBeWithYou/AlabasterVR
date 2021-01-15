// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-04-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-04-2021
// ***********************************************************************
// <copyright file="MessageBox.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class MessageBox.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class MessageBox : MonoBehaviour
{
    /// <summary>
    /// The text
    /// </summary>
    public Text text;

    /// <summary>
    /// Initializes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Init(string message)
    {
        text.text = message;
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
