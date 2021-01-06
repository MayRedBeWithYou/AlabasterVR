using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftOverlay : MonoBehaviour
{
    public Image undoImage;
    public Image redoImage;

    public Color validColor;
    public Color invalidColor;

    void Start()
    {
        OperationManager.Instance.OnStacksChanged += RefreshIcons;

        RefreshIcons();
    }

    void OnEnable()
    {
        RefreshIcons();
    }

    void RefreshIcons()
    {
        if (OperationManager.Instance.undoOperations.Count > 0) undoImage.color = validColor;
        else undoImage.color = invalidColor;

        if (OperationManager.Instance.redoOperations.Count > 0) redoImage.color = validColor;
        else redoImage.color = invalidColor;
    }
}
