// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-11-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-11-2020
// ***********************************************************************
// <copyright file="KeyboardKey.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class KeyboardKey.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class KeyboardKey : MonoBehaviour
{
    [SerializeField]
    private Keyboard _keyboard;

    Button _button;
    InputField _inputField;
    /// <summary>
    /// The primary
    /// </summary>
    public char Primary;
    /// <summary>
    /// The secondary
    /// </summary>
    public char Secondary;
    /// <summary>
    /// The alt primary
    /// </summary>
    public char AltPrimary;
    /// <summary>
    /// The alt secondary
    /// </summary>
    public char AltSecondary;

    private Text currentText;

    /// <summary>
    /// Gets a value indicating whether this instance is letter.
    /// </summary>
    /// <value><c>true</c> if this instance is letter; otherwise, <c>false</c>.</value>
    public bool IsLetter => Primary >= 'a' && Primary <= 'z';

    void Start()
    {
        _button = GetComponent<Button>();
        currentText = GetComponentInChildren<Text>();

        _button.onClick.AddListener(OnClick);
        _inputField = _keyboard.inputField;
    }

    /// <summary>
    /// Change2s the primary.
    /// </summary>
    public void Change2Primary()
    {
        currentText.text = Primary.ToString();
    }

    /// <summary>
    /// Change2s the secondary.
    /// </summary>
    public void Change2Secondary()
    {
        currentText.text = Secondary.ToString();
    }

    /// <summary>
    /// Change2s the alt primary.
    /// </summary>
    public void Change2AltPrimary()
    {
        currentText.text = AltPrimary.ToString();
    }

    /// <summary>
    /// Change2s the alt secondary.
    /// </summary>
    public void Change2AltSecondary()
    {
        currentText.text = AltSecondary.ToString();
    }

    /// <summary>
    /// Called when [click].
    /// </summary>
    public void OnClick()
    {
        if (_inputField.text.Length == _inputField.caretPosition)
        {
            _inputField.text += currentText.text;
            _inputField.caretPosition++;
            _inputField.ForceLabelUpdate();
        }
        else
        {
            string text = _inputField.text.Substring(0, _inputField.caretPosition) + currentText.text;
            text += _inputField.text.Substring(_inputField.caretPosition, _inputField.text.Length - _inputField.caretPosition);
            _inputField.text = text;
            _inputField.caretPosition++;
            _inputField.ForceLabelUpdate();
        }

    }
}