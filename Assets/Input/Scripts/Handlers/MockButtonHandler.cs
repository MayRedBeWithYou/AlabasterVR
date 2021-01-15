// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-03-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-03-2020
// ***********************************************************************
// <copyright file="MockButtonHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class MockButtonHandler.
/// Implements the <see cref="ButtonHandler" />
/// </summary>
/// <seealso cref="ButtonHandler" />
[CreateAssetMenu(menuName = "Input/MockButton", fileName = "NewMockButtonHandler")]
public class MockButtonHandler : ButtonHandler
{
    /// <summary>
    /// The mocked button
    /// </summary>
    public ButtonHandler mockedButton;
    /// <summary>
    /// The key
    /// </summary>
    public KeyCode key;

    /// <summary>
    /// Handles the state.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public override void HandleState(XRController controller)
    {
        if (Input.GetKeyDown(key))
        {
            if (!previousPress)
            {
                previousPress = true;
                mockedButton.InvokeDownEvent(controller);
            }    
        }
        if (Input.GetKeyUp(key))
        {
            if (previousPress)
            {
                previousPress = false;
                mockedButton.InvokeUpEvent(controller);
            }
        }
    }
}