using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefPictureManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject PicturesHolder;
    private static RefPictureManager _instance;
    public static RefPictureManager Instance { get { return _instance; } }
    public static int counter = 0;
    public List<PictureItem> pictures = new List<PictureItem>();
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
            if (System.String.IsNullOrWhiteSpace(text)) return;
            PictureItem item = Instantiate(itemPrefab, PicturesHolder.transform).GetComponent<PictureItem>();
            item.Init(text, UIController.Instance.ShowPictureCanvas());
            pictures.Add(item);
            script.Close();
        };
    }
}
