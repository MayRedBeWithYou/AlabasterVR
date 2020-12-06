using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardKeySpecial : MonoBehaviour
{
    Button _button;

    InputField _inputField;

    [SerializeField]
    private Keyboard _keyboard;

    public SpecialKeyType Key;

    void Start()
    {
        _button = GetComponent<Button>();

        _inputField = _keyboard.inputField;

        if (Key != SpecialKeyType.Shift && Key != SpecialKeyType.Alt) _button.onClick.AddListener(OnClick);

    }
    public void OnClick()
    {
        switch (Key)
        {
            case SpecialKeyType.Backspace:
                if (_inputField.caretPosition > 0)
                {
                    string text = _inputField.text.Substring(0, _inputField.caretPosition - 1);
                    if (_inputField.caretPosition < _inputField.text.Length)
                    {
                        text += _inputField.text.Substring(_inputField.caretPosition, _inputField.text.Length - _inputField.caretPosition);
                        _inputField.caretPosition--;
                    }
                    _inputField.text = text;
                }
                break;
            case SpecialKeyType.Delete:
                if (_inputField.caretPosition <= _inputField.text.Length)
                {
                    string text = _inputField.text.Substring(0, _inputField.caretPosition);
                    if (_inputField.caretPosition < _inputField.text.Length - 1)
                    {
                        text += _inputField.text.Substring(_inputField.caretPosition + 1, _inputField.text.Length - _inputField.caretPosition - 1);
                    }
                    _inputField.text = text;
                }
                break;
            case SpecialKeyType.CapsLock:
                _keyboard.ApplyCapsLock();
                break;
            case SpecialKeyType.Shift:
                _keyboard.ApplyShift();
                break;
            case SpecialKeyType.Alt:
                _keyboard.ApplyAlt();
                break;
            case SpecialKeyType.Confirm:
                break;
            case SpecialKeyType.Cancel:
                break;
            case SpecialKeyType.Clear:
                _inputField.text = "";
                _inputField.caretPosition = 0;
                break;
            case SpecialKeyType.LeftCaret:
                if (_inputField.caretPosition > 0) _inputField.caretPosition--;
                break;
            case SpecialKeyType.RightCaret:
                if (_inputField.caretPosition <= _inputField.text.Length) _inputField.caretPosition++;
                break;
        }
    }
}
public enum SpecialKeyType
{
    Backspace, Delete, CapsLock, Shift, Alt, Confirm, Cancel, Clear, LeftCaret, RightCaret
}
