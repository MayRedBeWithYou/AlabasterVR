using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotteryUI : MonoBehaviour
{
    public PottersWheel wheel;

    public Sprite playImage;

    public Sprite pauseImage;
    
    public Button playButton;

    public Button resetButton;

    public Button hideButton;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (wheel.IsActive)
        {
            playButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pauseImage;
        }
        else playButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = playImage;

        if (wheel.IsHidden)
        {
            hideButton.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.black;
        }
        else
        {
            hideButton.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.gray;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
