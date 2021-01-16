using HSVPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerKeyboardPopup : MonoBehaviour
{
    ColorPicker picker;
    Keyboard keyboard;

    public void Awake()
    {
        picker = GetComponent<ColorPicker>();
    }

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
