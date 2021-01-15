// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-03-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-03-2020
// ***********************************************************************
// <copyright file="ButtonHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class ButtonHandler.
/// Implements the <see cref="InputHandler" />
/// </summary>
/// <seealso cref="InputHandler" />
[CreateAssetMenu(menuName = "Input/Button", fileName = "NewButtonHandler")]
public class ButtonHandler : InputHandler
{
    /// <summary>
    /// Gets a value indicating whether this instance is pressed.
    /// </summary>
    /// <value><c>true</c> if this instance is pressed; otherwise, <c>false</c>.</value>
    public bool IsPressed => previousPress;

    /// <summary>
    /// The button
    /// </summary>
    public InputHelpers.Button button = InputHelpers.Button.None;

    /// <summary>
    /// Delegate StateChange
    /// </summary>
    /// <param name="controller">The controller.</param>
    public delegate void StateChange(XRController controller);

    /// <summary>
    /// Occurs when button is pressed.
    /// </summary>
    public event StateChange OnButtonDown;
    /// <summary>
    /// Occurs when button is released.
    /// </summary>
    public event StateChange OnButtonUp;

    protected bool previousPress = false;

    /// <summary>
    /// Handles the state.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public override void HandleState(XRController controller)
    {
        if (controller.inputDevice.IsPressed(button, out bool isPressed, controller.axisToPressThreshold))
        {
            if (previousPress != isPressed)
            {
                previousPress = isPressed;

                if (isPressed) OnButtonDown?.Invoke(controller);
                else OnButtonUp?.Invoke(controller);
            }
        }
    }

    /// <summary>
    /// Invokes OnButtonDown event.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public void InvokeDownEvent(XRController controller)
    {
        previousPress = true;
        OnButtonDown?.Invoke(controller);
    }

    /// <summary>
    /// Invokes OnButtonUp event.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public void InvokeUpEvent(XRController controller)
    {
        previousPress = false;
        OnButtonUp?.Invoke(controller);
    }
}
