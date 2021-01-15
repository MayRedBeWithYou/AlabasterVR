// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-06-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-06-2020
// ***********************************************************************
// <copyright file="EventPointerDown.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class EventPointerDown.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class EventPointerDown : MonoBehaviour
{
    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// Called when [pointer down delegate].
    /// </summary>
    /// <param name="data">The data.</param>
    public void OnPointerDownDelegate(PointerEventData data)
    {
        gameObject.GetComponent<KeyboardKeySpecial>().OnClick();
    }
}
