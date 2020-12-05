using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardKeySpecial : MonoBehaviour
{
    Button button;
    UnityEngine.UI.InputField inputField;
    KeyboardMain sc;
    public SpecialKeyType Key;
    // Start is called before the first frame update
    void Start()
    {
        button=this.gameObject.GetComponent<Button>();
        
        inputField=button.gameObject.transform.parent.parent.parent.GetChild(0).GetChild(0).gameObject.GetComponent<InputField>();
        sc=button.gameObject.transform.parent.parent.parent.parent.parent.gameObject.GetComponent<KeyboardMain>();
    
        if(Key!=SpecialKeyType.Shift && Key!=SpecialKeyType.Alt) button.onClick.AddListener(OnClick);
        
    }
    public void OnClick()
    {
        switch (Key)
        {
            case SpecialKeyType.Backspace:
                if(inputField.caretPosition>0)
                {
                    string tmp=inputField.text.Substring(0,inputField.caretPosition-1);
                    if(inputField.caretPosition<inputField.text.Length)
                    {
                        tmp+= inputField.text.Substring(inputField.caretPosition,inputField.text.Length-inputField.caretPosition);
                        inputField.caretPosition--;
                    }
                    inputField.text=tmp;
                }
                break;
            case SpecialKeyType.Delete:
                if(inputField.caretPosition<=inputField.text.Length)
                {
                    string tmp=inputField.text.Substring(0,inputField.caretPosition);
                    if(inputField.caretPosition<inputField.text.Length-1)
                    {
                        tmp+= inputField.text.Substring(inputField.caretPosition+1,inputField.text.Length-inputField.caretPosition-1);
                    }
                    inputField.text=tmp;
                }
                break;
            case SpecialKeyType.CapsLock:
                sc.ApplyCapsLock();
                break;
            case SpecialKeyType.Shift:
                sc.ApplyShift();
                break;
            case SpecialKeyType.Alt:
                sc.ApplyAlt();
                break;
            case SpecialKeyType.Confirm:
                break;
            case SpecialKeyType.Cancel:
                break;
            case SpecialKeyType.Clear:
                inputField.text="";
                inputField.caretPosition=0;
                break;
            case SpecialKeyType.LeftCaret:
                if(inputField.caretPosition>0) inputField.caretPosition--;
                break;
            case SpecialKeyType.RightCaret:
                if(inputField.caretPosition<=inputField.text.Length)inputField.caretPosition++;
                break;
        }
    }
}
public enum SpecialKeyType
{
    Backspace, Delete, CapsLock, Shift, Alt, Confirm, Cancel, Clear, LeftCaret, RightCaret
}
