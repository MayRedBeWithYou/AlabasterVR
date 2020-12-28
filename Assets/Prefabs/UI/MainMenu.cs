using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MainMenu : MonoBehaviour
{
    public Button NewFileButton;

    public Button SaveButton;

    public Button ImportButton;

    public Button OptionsButton;

    public Button ExitButton;

    public void Awake()
    {
        ExitButton.onClick.AddListener(Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

}