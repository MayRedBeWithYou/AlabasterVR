using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardKey : MonoBehaviour
{
    [SerializeField]
    private Keyboard _keyboard;

    Button _button;
    InputField _inputField;
    public char Primary;
    public char Secondary;
    public char AltPrimary;
    public char AltSecondary;

    private Text currentText;
    
    public bool IsLetter => Primary >= 'a' && Primary <= 'z';

    void Start()
    {
        _button = GetComponent<Button>();
        currentText = GetComponentInChildren<Text>();

        _button.onClick.AddListener(OnClick);
        _inputField = _keyboard.inputField;
    }

    public void Change2Primary()
    {
        currentText.text = Primary.ToString();
    }

    public void Change2Secondary()
    {
        currentText.text = Secondary.ToString();
    }

    public void Change2AltPrimary()
    {
        currentText.text = AltPrimary.ToString();
    }

    public void Change2AltSecondary()
    {
        currentText.text = AltSecondary.ToString();
    }

    public void OnClick()
    {
        if (_inputField.text.Length == _inputField.caretPosition)
        {
            _inputField.text += currentText.text;
            _inputField.caretPosition++;
        }
        else
        {
            string text = _inputField.text.Substring(0, _inputField.caretPosition) + currentText.text;
            text += _inputField.text.Substring(_inputField.caretPosition, _inputField.text.Length - _inputField.caretPosition);
            _inputField.text = text;
            _inputField.caretPosition++;
        }

    }
}