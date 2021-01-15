// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-03-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-03-2020
// ***********************************************************************
// <copyright file="MockAxis2DHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class MockAxis2DHandler.
/// Implements the <see cref="AxisHandler2D" />
/// </summary>
/// <seealso cref="AxisHandler2D" />
[CreateAssetMenu(menuName = "Input/MockAxis2D", fileName = "NewMockAxis2DHandler")]
public class MockAxis2DHandler : AxisHandler2D
{
    /// <summary>
    /// The mocked axis2 d
    /// </summary>
    public AxisHandler2D mockedAxis2D;
    /// <summary>
    /// The key up
    /// </summary>
    public KeyCode keyUp;
    /// <summary>
    /// The key down
    /// </summary>
    public KeyCode keyDown;
    /// <summary>
    /// The key left
    /// </summary>
    public KeyCode keyLeft;
    /// <summary>
    /// The key right
    /// </summary>
    public KeyCode keyRight;
    /// <summary>
    /// The mock value
    /// </summary>
    public float mockValue = 1f;

    /// <summary>
    /// Handles the state.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public override void HandleState(XRController controller)
    {
        Vector2 value = Vector2.zero;
        if (Input.GetKey(keyUp)) value.y += mockValue;
        if (Input.GetKey(keyDown)) value.y -= mockValue;
        if (Input.GetKey(keyLeft)) value.x -= mockValue;
        if (Input.GetKey(keyRight)) value.x += mockValue;
        if (previousValue != value) mockedAxis2D.InvokeEvent(controller, value);
        previousValue = value;
    }
}