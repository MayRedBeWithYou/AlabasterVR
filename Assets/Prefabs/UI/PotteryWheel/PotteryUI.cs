// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="PotteryUI.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class PotteryUI.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class PotteryUI : MonoBehaviour
{
    /// <summary>
    /// The wheel
    /// </summary>
    public PottersWheel wheel;

    /// <summary>
    /// The play image
    /// </summary>
    public Sprite playImage;

    /// <summary>
    /// The pause image
    /// </summary>
    public Sprite pauseImage;

    /// <summary>
    /// The play button
    /// </summary>
    public Button playButton;

    /// <summary>
    /// The reverse button
    /// </summary>
    public Button reverseButton;
    /// <summary>
    /// The forward button
    /// </summary>
    public Button forwardButton;

    /// <summary>
    /// The hide button
    /// </summary>
    public Button hideButton;

    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// Refreshes the UI.
    /// </summary>
    public void RefreshUI()
    {
        if (wheel.IsActive)
        {
            playButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pauseImage;
        }
        else playButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = playImage;

        if (wheel.IsHidden)
        {
            hideButton.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.black;
        }
        else
        {
            hideButton.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.gray;
        }
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
