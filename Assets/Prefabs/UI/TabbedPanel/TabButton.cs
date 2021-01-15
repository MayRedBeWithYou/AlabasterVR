// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="TabButton.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class TabButton.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="UnityEngine.EventSystems.IPointerEnterHandler" />
/// Implements the <see cref="UnityEngine.EventSystems.IPointerClickHandler" />
/// Implements the <see cref="UnityEngine.EventSystems.IPointerExitHandler" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="UnityEngine.EventSystems.IPointerEnterHandler" />
/// <seealso cref="UnityEngine.EventSystems.IPointerClickHandler" />
/// <seealso cref="UnityEngine.EventSystems.IPointerExitHandler" />
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    /// <summary>
    /// The tab group
    /// </summary>
    public TabbedPanel tabGroup;

    /// <summary>
    /// The panel
    /// </summary>
    public GameObject panel;

    /// <summary>
    /// The background
    /// </summary>
    public Image background;

    void Awake()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    /// <summary>
    /// Use this callback to detect clicks.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    /// <summary>
    /// Use this callback to detect pointer enter events
    /// </summary>
    /// <param name="eventData">The event data.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    /// <summary>
    /// Use this callback to detect pointer exit events
    /// </summary>
    /// <param name="eventData">The event data.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }
}
