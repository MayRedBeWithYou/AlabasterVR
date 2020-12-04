using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class KeyboardMain : MonoBehaviour
{
    public NumericalKeyMode NumericalKeyCurrentMode;
    public LetterKeyMode LetterKeyCurrentMode;

    // Start is called before the first frame update
    void Start()
    {

        InputField inputField = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<InputField>();
        inputField.caretPosition = 0; // desired cursor position
 
        inputField.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(inputField, true);
        inputField.GetType().InvokeMember("SetCaretVisible", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, inputField, null);

        var keyboardPanelTransform=this.gameObject.transform.GetChild(0).GetChild(0).GetChild(1);
    }

    public void ApplyShift()
    {
        var keyScriptsL=LetterKeys();
        switch(LetterKeyCurrentMode)
        {
            case LetterKeyMode.Primary:
                foreach (KeyboardKey s in keyScriptsL) s.Change2Secondary();
                LetterKeyCurrentMode=LetterKeyMode.Secondary;
                break;
            case LetterKeyMode.Secondary:
                foreach (KeyboardKey s in keyScriptsL) s.Change2Primary();
                LetterKeyCurrentMode=LetterKeyMode.Primary;
                break;
            case LetterKeyMode.AltPrimary:
                foreach (KeyboardKey s in keyScriptsL) s.Change2AltSecondary();
                LetterKeyCurrentMode=LetterKeyMode.AltSecondary;
                break;
            case LetterKeyMode.AltSecondary:
                foreach (KeyboardKey s in keyScriptsL) s.Change2AltPrimary();
                LetterKeyCurrentMode=LetterKeyMode.AltPrimary;
                break;
        }
        var keyScriptsN=NumericalKeys();
        if(NumericalKeyCurrentMode==NumericalKeyMode.Primary)
        {
            foreach (KeyboardKey s in keyScriptsN) s.Change2Secondary();
            NumericalKeyCurrentMode=NumericalKeyMode.Secondary;
        }
        else
        {
            foreach (KeyboardKey s in keyScriptsN) s.Change2Primary();
            NumericalKeyCurrentMode=NumericalKeyMode.Primary;
        }
    }
    public void ApplyCapsLock()
    {
        var keyScripts=LetterKeys();
        switch(LetterKeyCurrentMode)
        {
            case LetterKeyMode.Primary:
                foreach (KeyboardKey s in keyScripts) s.Change2Secondary();
                LetterKeyCurrentMode=LetterKeyMode.Secondary;
                break;
            case LetterKeyMode.Secondary:
                foreach (KeyboardKey s in keyScripts) s.Change2Primary();
                LetterKeyCurrentMode=LetterKeyMode.Primary;
                break;
            case LetterKeyMode.AltPrimary:
                foreach (KeyboardKey s in keyScripts) s.Change2AltSecondary();
                LetterKeyCurrentMode=LetterKeyMode.AltSecondary;
                break;
            case LetterKeyMode.AltSecondary:
                foreach (KeyboardKey s in keyScripts) s.Change2AltPrimary();
                LetterKeyCurrentMode=LetterKeyMode.AltPrimary;
                break;
        }
    }
    public void ApplyAlt()
    {
        var keyScripts=LetterKeys();
        switch(LetterKeyCurrentMode)
        {
            case LetterKeyMode.Primary:
                foreach (KeyboardKey s in keyScripts) s.Change2AltPrimary();
                LetterKeyCurrentMode=LetterKeyMode.AltPrimary;
                break;
            case LetterKeyMode.Secondary:
                foreach (KeyboardKey s in keyScripts) s.Change2AltSecondary();
                LetterKeyCurrentMode=LetterKeyMode.AltSecondary;
                break;
            case LetterKeyMode.AltPrimary:
                foreach (KeyboardKey s in keyScripts) s.Change2Primary();
                LetterKeyCurrentMode=LetterKeyMode.Primary;
                break;
            case LetterKeyMode.AltSecondary:
                foreach (KeyboardKey s in keyScripts) s.Change2Secondary();
                LetterKeyCurrentMode=LetterKeyMode.Secondary;
                break;
        }
    }
    List<KeyboardKey> NumericalKeys()
    {
        var keyboardPanelTransform=this.gameObject.transform.GetChild(0).GetChild(0).GetChild(1);
        List<KeyboardKey> result=new List<KeyboardKey>();
        for(int i=0;i<13;i++)
        {
            result.Add(keyboardPanelTransform.GetChild(0).GetChild(i).gameObject.GetComponent<KeyboardKey>());
        }
        result.Add(keyboardPanelTransform.GetChild(1).GetChild(11).gameObject.GetComponent<KeyboardKey>());
        result.Add(keyboardPanelTransform.GetChild(1).GetChild(12).gameObject.GetComponent<KeyboardKey>());
        result.Add(keyboardPanelTransform.GetChild(1).GetChild(13).gameObject.GetComponent<KeyboardKey>());

        result.Add(keyboardPanelTransform.GetChild(2).GetChild(10).gameObject.GetComponent<KeyboardKey>());
        result.Add(keyboardPanelTransform.GetChild(2).GetChild(11).gameObject.GetComponent<KeyboardKey>());
        
        result.Add(keyboardPanelTransform.GetChild(3).GetChild(8).gameObject.GetComponent<KeyboardKey>());
        result.Add(keyboardPanelTransform.GetChild(3).GetChild(9).gameObject.GetComponent<KeyboardKey>());
        result.Add(keyboardPanelTransform.GetChild(3).GetChild(10).gameObject.GetComponent<KeyboardKey>());

        return result;
    }
    List<KeyboardKey> LetterKeys()
    {
        var keyboardPanelTransform=this.gameObject.transform.GetChild(0).GetChild(0).GetChild(1);
        List<KeyboardKey> result=new List<KeyboardKey>();
        for(int i=1;i<11;i++)
        {
            result.Add(keyboardPanelTransform.GetChild(1).GetChild(i).gameObject.GetComponent<KeyboardKey>());
        }
        for(int i=1;i<10;i++)
        {
            result.Add(keyboardPanelTransform.GetChild(2).GetChild(i).gameObject.GetComponent<KeyboardKey>());
        }
        for(int i=1;i<8;i++)
        {
            result.Add(keyboardPanelTransform.GetChild(3).GetChild(i).gameObject.GetComponent<KeyboardKey>());
        }
        return result;
    }
}
public enum NumericalKeyMode
{
    Primary, Secondary
}
public enum LetterKeyMode
{
    Primary, Secondary, AltPrimary,AltSecondary
}