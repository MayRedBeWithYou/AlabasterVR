using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class PictureItem : MonoBehaviour
{
    [SerializeField]
    Button hideButton;
    Image hideImage;
    [SerializeField]
    Text picName;
    PictureCanvas pictureCanvas;
    public string PicName { get { return picName.text; } set { picName.text = value; } }
    bool visible;
    public void Close()
    {
        if (pictureCanvas != null)
        {
            pictureCanvas.Close();
            pictureCanvas = null;
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    public void Hide()
    {
        if (pictureCanvas != null) pictureCanvas.Hide();
        else ChangeVisibility();
    }
    public void ChangeName()
    {
        if (pictureCanvas != null) pictureCanvas.ChangeName();
    }
    public void ChangeVisibility()
    {
        visible = !visible;
        if (visible) hideImage.color = Color.white;
        else hideImage.color = Color.black;
    }
    public void Init(string path, PictureCanvas canvas)
    {
        visible = true;
        pictureCanvas = canvas;
        pictureCanvas.Init(path, this);
        hideImage = hideButton.GetComponentInChildren<Image>();
    }
}
