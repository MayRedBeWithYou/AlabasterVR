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
    public PictureItem pictureItem;
    public string PicName { get { return picName.text; } 
    set { 
            picName.text = value; 
            if(pictureItem!=null)pictureItem.PicName=value;
        }}

    public void Init(string path, PictureItem pictureItem)
    {
        this.path = path;
        this.pictureItem = pictureItem;
        LoadImage(path);
        PicName = Path.GetFileNameWithoutExtension(path);
    }
    public void UpdateImage()
    {
        var script = UIController.Instance.ShowRefPicture();
        script.OnAccepted += (text) =>
        {
            if (System.String.IsNullOrWhiteSpace(text)) return;
            Init(text,this.pictureItem);
            script.Close();
        };
    }
    public void Close()
    {
        if (pictureItem != null)
        {
            RefPictureManager.Instance.pictures.Remove(pictureItem);
            pictureItem.gameObject.SetActive(false);
            Destroy(pictureItem.gameObject);
            pictureItem = null;
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    public void Hide()
    {
        if (pictureItem != null) pictureItem.ChangeVisibility();
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void ChangeName()
    {
        Keyboard keyboard = Keyboard.Show(PicName);
        keyboard.OnAccepted += (text) =>
        {
            PicName = text;
            keyboard.Close();
        };
        keyboard.OnCancelled += () => keyboard.Close();
    }
    public void LoadImage(string path)
    {
        image.sprite = LoadNewSprite(path);
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
