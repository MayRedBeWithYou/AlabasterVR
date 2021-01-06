using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class YesNoCancelPopup : MonoBehaviour
{
    public Text text;

    public delegate void OnState();

    public event OnState OnAccept;

    public event OnState OnDecline;

    public event OnState OnCancel;

    public void Init(string message)
    {
        text.text = message;
    }

    public void Accept()
    {
        OnAccept?.Invoke();
        Close();
    }

    public void Decline()
    {
        OnDecline?.Invoke();
        Close();
    }

    public void Cancel()
    {
        OnCancel?.Invoke();
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
