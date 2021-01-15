// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="Tool.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class Tool.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public abstract class Tool : MonoBehaviour
{
    /// <summary>
    /// Enables this instance.
    /// </summary>
    public virtual void Enable()
    {
        gameObject.SetActive(true);
        if (cursor != null)
            cursor.gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables this instance.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the maximum size.
    /// </summary>
    /// <value>The maximum size.</value>
    public float MaxSize
    {
        get => _maxSize;
        protected set
        {
            _maxSize = value;
            cursor.MaximalSize = value;
        }
    }
    /// <summary>
    /// Gets or sets the minimum size.
    /// </summary>
    /// <value>The minimum size.</value>
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
    /// <summary>
    /// The tool name
    /// </summary>
    public string toolName;

    /// <summary>
    /// The description
    /// </summary>
    public string description;

    /// <summary>
    /// The sprite
    /// </summary>
    public Sprite sprite;
}
