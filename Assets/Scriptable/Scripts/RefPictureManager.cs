using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefPictureManager : MonoBehaviour
{
    public GameObject PictureCanvasPrefab;
    public GameObject PicturesHolder;

    private static RefPictureManager _instance;
    public static RefPictureManager Instance { get { return _instance; } }

    public delegate void PictureChanged(PictureCanvas canvas);

    public static event PictureChanged PictureAdded;
    public static event PictureChanged PictureRemoved;
    public static event PictureChanged PictureHidden;
    public static event PictureChanged PictureNameUpdated;

    public List<PictureCanvas> pictures = new List<PictureCanvas>();
    void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }
    public void AddRefPicture()
    {
        var script = UIController.Instance.ShowRefPicture();
        script.OnAccepted += (text) =>
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            PictureCanvas canvas = UIController.Instance.ShowPictureCanvas();
            pictures.Add(canvas);
            canvas.SetPicture(text);
            PictureAdded?.Invoke(canvas);
            script.Close();
        };
        
    }
    public void RemovePicture(PictureCanvas canvas)
    {
        pictures.Remove(canvas);
        canvas.gameObject.SetActive(false);
        Destroy(canvas.gameObject);
        PictureRemoved?.Invoke(canvas);
    }
    public void HidePicture(PictureCanvas canvas)
    {
        canvas.visible = !canvas.visible;
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        PictureHidden?.Invoke(canvas);
    }
    public void ChangeName(PictureCanvas canvas)
    {
        Keyboard keyboard = Keyboard.Show(canvas.PicName);
        keyboard.OnAccepted += (text) =>
        {
            UpdateName(canvas, text);
            keyboard.Close();
        };
        keyboard.OnCancelled += () => keyboard.Close();
    }
    public void UpdateName(PictureCanvas canvas, string name)
    {
        canvas.PicName = name;
        PictureNameUpdated?.Invoke(canvas);
    }

}
