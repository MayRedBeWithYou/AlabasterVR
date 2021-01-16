using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefPicturesSelection : MonoBehaviour
{
    private static RefPicturesSelection _instance;
    public static RefPicturesSelection Instance { get { return _instance; } }
    public GameObject contentHolder;
    public GameObject refPictureItemPrefab;
    public Dictionary<PictureCanvas, PictureItem> pictures;

    public void Start()
    {
        pictures = new Dictionary<PictureCanvas, PictureItem>();

        foreach (PictureCanvas canvas in RefPictureManager.Instance.pictures)
        {
            PictureAdded(canvas);
        }
        RefPictureManager.PictureAdded+=PictureAdded;
        RefPictureManager.PictureRemoved+=PictureRemoved;
        RefPictureManager.PictureHidden+=PicturesHidden;
        RefPictureManager.PictureNameUpdated+=PictureNameUpdated;
    }

    private void PictureRemoved(PictureCanvas canvas)
    {
        PictureItem item = pictures[canvas];
        pictures.Remove(canvas);
        item.gameObject.SetActive(false);
        Destroy(item.gameObject);
    }

    private void PictureAdded(PictureCanvas canvas)
    {
        PictureItem item = Instantiate(refPictureItemPrefab, contentHolder.transform).GetComponent<PictureItem>();
        pictures.Add(canvas, item);
        item.canvas=canvas;
        item.ChangeVisibilityButton();
        PictureNameUpdated(canvas);
    }
    public void PicturesHidden(PictureCanvas canvas)
    {
        pictures[canvas].ChangeVisibilityButton();   
    }

    private void PictureNameUpdated(PictureCanvas canvas)
    {
        pictures[canvas].PicName=canvas.PicName;
    }
    public void OnDestroy()
    {
        RefPictureManager.PictureAdded-=PictureAdded;
        RefPictureManager.PictureRemoved-=PictureRemoved;
        RefPictureManager.PictureHidden-=PicturesHidden;
        RefPictureManager.PictureNameUpdated-=PictureNameUpdated;
    }
}
