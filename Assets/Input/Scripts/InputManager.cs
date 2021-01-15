// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-04-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-04-2021
// ***********************************************************************
// <copyright file="InputManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class InputManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// The button handlers
    /// </summary>
    public List<ButtonHandler> buttonHandlers = new List<ButtonHandler>();

    /// <summary>
    /// The axis handlers
    /// </summary>
    public List<AxisHandler> axisHandlers = new List<AxisHandler>();

    /// <summary>
    /// The axis handler2 ds
    /// </summary>
    public List<AxisHandler2D> axisHandler2Ds = new List<AxisHandler2D>();

    /// <summary>
    /// The controller
    /// </summary>
    public XRController controller;

    private OnHoverEventHandler onHoverHandler = null;


    private void Awake()
    {
        onHoverHandler = GetComponent<OnHoverEventHandler>();
    }

    private void Update()
    {
        HandleButtonEvents();
        HandleAxis2DEvents();
        HandleAxisEvents();
    }

    private void HandleButtonEvents()
    {
        foreach (ButtonHandler handler in buttonHandlers)
            handler.HandleState(controller);
    }

    private void HandleAxis2DEvents()
    {
        foreach (AxisHandler2D handler in axisHandler2Ds)
            handler.HandleState(controller);
    }

    /// <summary>
    /// Handles the axis events.
    /// </summary>
    public void HandleAxisEvents()
    {
        foreach (AxisHandler handler in axisHandlers)
            handler.HandleState(controller);
    }
}

