using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class PictureCanvas : MonoBehaviour
{
    [SerializeField]
    private Text picName;
    [SerializeField]
    private Image image;
    private string path;
    [HideInInspector]
    public string PicName { get { return picName.text; } set { picName.text = value;}}
    public bool visible;

    public void UpdateImage()
    {
        var script = UIController.Instance.ShowRefPicture();
        script.OnAccepted += (text) =>
        {
            if (System.String.IsNullOrWhiteSpace(text)) return;
            SetPicture(text);
            script.Close();
        };
    }
    public void SetPicture(string text)
    {
        path=text;
        PicName=Path.GetFileNameWithoutExtension(text);
        image.sprite = LoadNewSprite(text);
    }
    public void Close()
    {
        RefPictureManager.Instance.RemovePicture(this);
    }
    public void Hide()
    {
        RefPictureManager.Instance.HidePicture(this);
    }
    public void ChangeName()
    {
        RefPictureManager.Instance.ChangeName(this);
    }
    private Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        //https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/        

        Texture2D texture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

        return NewSprite;
    }

    private Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(1, 1);
            if (Tex2D.LoadImage(FileData)) return Tex2D;
            else return null;
        }
        return null;
    }
}
