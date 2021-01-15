// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-08-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-08-2021
// ***********************************************************************
// <copyright file="Keyboard.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

/// <summary>
/// Class Keyboard.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class Keyboard : MonoBehaviour
{
    /// <summary>
    /// The numerical key current mode
    /// </summary>
    public NumericalKeyMode NumericalKeyCurrentMode;
    /// <summary>
    /// The letter key current mode
    /// </summary>
    public LetterKeyMode LetterKeyCurrentMode;

    /// <summary>
    /// The accept button
    /// </summary>
    public Button acceptButton;
    /// <summary>
    /// The cancel button
    /// </summary>
    public Button cancelButton;

    /// <summary>
    /// The input field
    /// </summary>
    public InputField inputField;

    [SerializeField]
    private List<KeyboardKey> letterKeys;

    [SerializeField]
    private List<KeyboardKey> numKeys;

    /// <summary>
    /// Delegate InputAccepted
    /// </summary>
    /// <param name="text">The text.</param>
    public delegate void InputAccepted(string text);
    /// <summary>
    /// Occurs when [on accepted].
    /// </summary>
    public event InputAccepted OnAccepted;

    /// <summary>
    /// Delegate InputCancelled
    /// </summary>
    public delegate void InputCancelled();
    /// <summary>
    /// Occurs when [on cancelled].
    /// </summary>
    public event InputCancelled OnCancelled;
    /// <summary>
    /// Occurs when [on closing].
    /// </summary>
    public event InputCancelled OnClosing;

    private bool isSet = false;

    void Start()
    {
        inputField.caretPosition = 0; // desired cursor position

        inputField.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(inputField, true);
        inputField.GetType().InvokeMember("SetCaretVisible", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, inputField, null);

        acceptButton.onClick.AddListener(() => OnAccepted?.Invoke(inputField.text));
        cancelButton.onClick.AddListener(() => OnCancelled?.Invoke());
    }

    /// <summary>
    /// Sets the text.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetText(string text)
    {
        inputField.text = text;
        isSet = true;
        inputField.ForceLabelUpdate();
    }

    /// <summary>
    /// Lates the update.
    /// </summary>
    public void LateUpdate()
    {
        if (isSet)
        {
            isSet = false;
            inputField.caretPosition = inputField.text.Length;
            inputField.ForceLabelUpdate();
        }
    }

    /// <summary>
    /// Applies the shift.
    /// </summary>
    public void ApplyShift()
    {
        switch (LetterKeyCurrentMode)
        {
            case LetterKeyMode.Primary:
                foreach (KeyboardKey s in letterKeys) s.Change2Secondary();
                LetterKeyCurrentMode = LetterKeyMode.Secondary;
                break;
            case LetterKeyMode.Secondary:
                foreach (KeyboardKey s in letterKeys) s.Change2Primary();
                LetterKeyCurrentMode = LetterKeyMode.Primary;
                break;
            case LetterKeyMode.AltPrimary:
                foreach (KeyboardKey s in letterKeys) s.Change2AltSecondary();
                LetterKeyCurrentMode = LetterKeyMode.AltSecondary;
                break;
            case LetterKeyMode.AltSecondary:
                foreach (KeyboardKey s in letterKeys) s.Change2AltPrimary();
                LetterKeyCurrentMode = LetterKeyMode.AltPrimary;
                break;
        }
        if (NumericalKeyCurrentMode == NumericalKeyMode.Primary)
        {
            foreach (KeyboardKey s in numKeys) s.Change2Secondary();
            NumericalKeyCurrentMode = NumericalKeyMode.Secondary;
        }
        else
        {
            foreach (KeyboardKey s in numKeys) s.Change2Primary();
            NumericalKeyCurrentMode = NumericalKeyMode.Primary;
        }
    }

    /// <summary>
    /// Applies the caps lock.
    /// </summary>
    public void ApplyCapsLock()
    {
        switch (LetterKeyCurrentMode)
        {
            case LetterKeyMode.Primary:
                foreach (KeyboardKey s in letterKeys) s.Change2Secondary();
                LetterKeyCurrentMode = LetterKeyMode.Secondary;
                break;
            case LetterKeyMode.Secondary:
                foreach (KeyboardKey s in letterKeys) s.Change2Primary();
                LetterKeyCurrentMode = LetterKeyMode.Primary;
                break;
            case LetterKeyMode.AltPrimary:
                foreach (KeyboardKey s in letterKeys) s.Change2AltSecondary();
                LetterKeyCurrentMode = LetterKeyMode.AltSecondary;
                break;
            case LetterKeyMode.AltSecondary:
                foreach (KeyboardKey s in letterKeys) s.Change2AltPrimary();
                LetterKeyCurrentMode = LetterKeyMode.AltPrimary;
                break;
        }
    }

    /// <summary>
    /// Applies the alt.
    /// </summary>
    public void ApplyAlt()
    {
        switch (LetterKeyCurrentMode)
        {
            case LetterKeyMode.Primary:
                foreach (KeyboardKey s in letterKeys) s.Change2AltPrimary();
                LetterKeyCurrentMode = LetterKeyMode.AltPrimary;
                break;
            case LetterKeyMode.Secondary:
                foreach (KeyboardKey s in letterKeys) s.Change2AltSecondary();
                LetterKeyCurrentMode = LetterKeyMode.AltSecondary;
                break;
            case LetterKeyMode.AltPrimary:
                foreach (KeyboardKey s in letterKeys) s.Change2Primary();
                LetterKeyCurrentMode = LetterKeyMode.Primary;
                break;
            case LetterKeyMode.AltSecondary:
                foreach (KeyboardKey s in letterKeys) s.Change2Secondary();
                LetterKeyCurrentMode = LetterKeyMode.Secondary;
                break;
        }
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        OnClosing?.Invoke();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Shows the specified parent.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="text">The text.</param>
    /// <returns>Keyboard.</returns>
    public static Keyboard Show(GameObject parent, string text = "")
    {
        return UIController.Instance.ShowKeyboard(parent, text);
    }

    /// <summary>
    /// Shows the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>Keyboard.</returns>
    public static Keyboard Show(string text = "")
    {
        return UIController.Instance.ShowKeyboard(text);
    }
}

public enum NumericalKeyMode
{
    /// <summary>
    /// The primary
    /// </summary>
    Primary, Secondary
}

public enum LetterKeyMode
{
    /// <summary>
    /// The primary
    /// </summary>
    Primary, Secondary, AltPrimary, AltSecondary
}