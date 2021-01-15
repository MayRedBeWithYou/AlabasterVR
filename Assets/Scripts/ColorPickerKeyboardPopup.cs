// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-07-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-07-2021
// ***********************************************************************
// <copyright file="ColorPickerKeyboardPopup.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ColorPickerKeyboardPopup.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class ColorPickerKeyboardPopup : MonoBehaviour
{
    ColorPicker picker;
    Keyboard keyboard;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    public void Awake()
    {
        picker = GetComponent<ColorPicker>();
    }

    /// <summary>
    /// Shows the keyboard.
    /// </summary>
    public void ShowKeyboard()
    {
        if (keyboard != null) return;
        keyboard = Keyboard.Show("#" + ColorUtility.ToHtmlStringRGB(picker.CurrentColor));
        keyboard.OnAccepted += (text) =>
        {
            if (ColorUtility.TryParseHtmlString(text, out Color color))
            {
                picker.CurrentColor = color;
                keyboard.Close();
            }
            else UIController.Instance.ShowMessageBox(keyboard.gameObject, "Color code invalid.\nHex code should be in format #RRGGBB.");
        };
        keyboard.OnCancelled += () => keyboard.Close();
    }
}
