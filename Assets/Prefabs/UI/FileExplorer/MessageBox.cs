using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MessageBox : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    public void Init(string message)
    {
        text.text=message;
    }
    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
