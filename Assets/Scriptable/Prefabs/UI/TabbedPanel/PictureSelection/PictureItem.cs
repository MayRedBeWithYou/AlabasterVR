using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class PictureItem : MonoBehaviour
{
    [SerializeField]
    Image hideImage;
    [SerializeField]
    Text picName;
    public PictureCanvas canvas;
    public string PicName { get { return picName.text; } set { picName.text = value; } }
    public void Close()
    {
        RefPictureManager.Instance.RemovePicture(canvas);
    }
    public void Hide()
    {
        RefPictureManager.Instance.HidePicture(canvas);
    }
    public void ChangeName()
    {
        RefPictureManager.Instance.ChangeName(canvas);
    }
    public void ChangeVisibilityButton()
    {
        if (canvas.visible) hideImage.color = Color.white;
        else hideImage.color = Color.black;
    }
}
