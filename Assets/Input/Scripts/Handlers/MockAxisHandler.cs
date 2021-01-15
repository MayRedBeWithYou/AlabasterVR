// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-03-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-03-2020
// ***********************************************************************
// <copyright file="MockAxisHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class MockAxisHandler.
/// Implements the <see cref="AxisHandler" />
/// </summary>
/// <seealso cref="AxisHandler" />
[CreateAssetMenu(menuName = "Input/MockAxis", fileName = "NewMockAxisHandler")]
public class MockAxisHandler : AxisHandler
{
    /// <summary>
    /// The mocked axis
    /// </summary>
    public AxisHandler mockedAxis;
    /// <summary>
    /// The key
    /// </summary>
    public KeyCode key;
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
        float value = previousValue;
        if (Input.GetKeyDown(key)) value = mockValue;
        if (Input.GetKeyUp(key)) value = 0f;
        if (previousValue != value) mockedAxis.InvokeEvent(controller, value);
        previousValue = value;
    }
}