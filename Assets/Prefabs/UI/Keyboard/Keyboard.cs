using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class Keyboard : MonoBehaviour
{
    public NumericalKeyMode NumericalKeyCurrentMode;
    public LetterKeyMode LetterKeyCurrentMode;

    public Button acceptButton;
    public Button cancelButton;

    public InputField inputField;

    [SerializeField]
    private List<KeyboardKey> letterKeys;

    [SerializeField]
    private List<KeyboardKey> numKeys;

    public delegate void InputAccepted(string text);
    public event InputAccepted OnAccepted;

    public delegate void InputCancelled();
    public event InputCancelled OnCancelled;
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

    public void SetText(string text)
    {
        inputField.text = text;
        isSet = true;
        inputField.ForceLabelUpdate();
    }

    public void LateUpdate()
    {
        if(isSet)
        {
            isSet = false;
            inputField.caretPosition = inputField.text.Length;
            inputField.ForceLabelUpdate();
        }
    }

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

    public void Close()
    {
        OnClosing?.Invoke();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public static Keyboard Show(string text = "")
    {
        return ToolController.Instance.ShowKeyboard(text);
    }
}

public enum NumericalKeyMode
{
    Primary, Secondary
}

public enum LetterKeyMode
{
    Primary, Secondary, AltPrimary, AltSecondary
}