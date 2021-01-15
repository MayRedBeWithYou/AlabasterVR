// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-06-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-06-2020
// ***********************************************************************
// <copyright file="EventPointerUp.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class EventPointerUp.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class EventPointerUp : MonoBehaviour
{
    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// Called when [pointer up delegate].
    /// </summary>
    /// <param name="data">The data.</param>
    public void OnPointerUpDelegate(PointerEventData data)
    {
        gameObject.GetComponent<KeyboardKeySpecial>().OnClick();
    }
}
