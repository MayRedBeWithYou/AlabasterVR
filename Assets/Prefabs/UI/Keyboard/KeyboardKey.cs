using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardKey : MonoBehaviour
{
    UnityEngine.UI.Button button;
    UnityEngine.UI.InputField inputField;
    public char Primary;
    public char Secondary;
    public char AltPrimary;
    public char AltSecondary;
    public bool IsLetter{get{return Primary>='a'&&Primary<='z';}}    

    // Start is called before the first frame update
    void Start()
    {
        button=this.gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        inputField=button.gameObject.transform.parent.parent.parent.GetChild(0).GetChild(0).gameObject.GetComponent<InputField>();
        if(Primary=='_'&&Secondary=='_') Primary=Secondary=AltPrimary=AltSecondary=' ';
    }
    public void Change2Primary()
    {
        button.GetComponentInChildren<Text>().text=Primary.ToString();
    }
    public void Change2Secondary()
    {
        button.GetComponentInChildren<Text>().text=Secondary.ToString();
    }
    public void Change2AltPrimary()
    {
        button.GetComponentInChildren<Text>().text=AltPrimary.ToString();
    }
    public void Change2AltSecondary()
    {
        button.GetComponentInChildren<Text>().text=AltSecondary.ToString();
    }

    public void OnClick()
    {
        if(inputField.text.Length==inputField.caretPosition)
        {
            inputField.text+=button.GetComponentInChildren<Text>().text;
            inputField.caretPosition++;
        }
        else
        {
            string tmp=inputField.text.Substring(0,inputField.caretPosition)+button.GetComponentInChildren<Text>().text;
            tmp+=inputField.text.Substring(inputField.caretPosition,inputField.text.Length-inputField.caretPosition);
            inputField.text=tmp;
            inputField.caretPosition++;
        }
        
    }
}