using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tool : MonoBehaviour
{
    public virtual void Enable()
    {
        gameObject.SetActive(true);
        if (cursor != null)
            cursor.gameObject.SetActive(true);
    }

    public virtual void Disable()
    {
        gameObject.SetActive(false);
        if (cursor != null)
            cursor.gameObject.SetActive(false);
    }

    protected virtual void Awake()
    {
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<BaseCursor>();
        cursor.gameObject.name = $"{toolName}Cursor";
        cursor.Color = defaultCursorColor;
    }

    protected virtual void Start()
    {
        SetMinSize();
        SetMaxSize();
        cursor.SetSizeToDefault();
    }
    private float _maxSize;
    private float _minSize;

    public float MaxSize
    {
        get => _maxSize;
        protected set
        {
            _maxSize = value;
            cursor.MaximalSize = value;
        }
    }
    public float MinSize
    {
        get => _minSize;
        protected set
        {
            _minSize = value;
            cursor.MinimalSize = value;
        }
    }

    protected abstract void SetMaxSize();
    protected abstract void SetMinSize();

    [SerializeField]
    protected BaseCursor cursorPrefab;
    protected BaseCursor cursor;

    [SerializeField]
    protected Color defaultCursorColor;
    public string toolName;

    public string description;

    public Sprite sprite;
}
