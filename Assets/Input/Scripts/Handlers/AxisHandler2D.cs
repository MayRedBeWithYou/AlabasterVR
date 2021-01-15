// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-03-2020
//
// Last Modified By : MayRe
// Last Modified On : 12-03-2020
// ***********************************************************************
// <copyright file="AxisHandler2D.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class AxisHandler2D.
/// Implements the <see cref="InputHandler" />
/// Implements the <see cref="UnityEngine.ISerializationCallbackReceiver" />
/// </summary>
/// <seealso cref="InputHandler" />
/// <seealso cref="UnityEngine.ISerializationCallbackReceiver" />
[CreateAssetMenu(menuName = "Input/Axis2D", fileName = "NewAxisHandler2D")]
public class AxisHandler2D : InputHandler, ISerializationCallbackReceiver
{
    public enum Axis2D
    {
        None,
        Primary2DAxis,
        Secondary2DAxis
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value represented as a Vector2.</value>
    public Vector2 Value => previousValue;

    /// <summary>
    /// Delegate ValueChange
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">The value.</param>
    public delegate void ValueChange(XRController controller, Vector2 value);
    /// <summary>
    /// Occurs when value changes.
    /// </summary>
    public event ValueChange OnValueChange;

    /// <summary>
    /// Handled axis.
    /// </summary>
    public Axis2D axis = Axis2D.None;

    private InputFeatureUsage<Vector2> inputFeature;
    protected Vector2 previousValue = Vector2.zero;

    /// <summary>
    /// Implement this method to receive a callback after Unity deserializes your object.
    /// </summary>
    public void OnAfterDeserialize()
    {
        inputFeature = new InputFeatureUsage<Vector2>(axis.ToString());
    }

    /// <summary>
    /// Implement this method to receive a callback before Unity serializes your object.
    /// </summary>
    public void OnBeforeSerialize() { }

    /// <summary>
    /// Handles the state.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public override void HandleState(XRController controller)
    {
        if(GetValue(controller, out Vector2 value))
        {
            if (value != previousValue)
            {
                previousValue = value;
                OnValueChange?.Invoke(controller, value);
            }
        }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">Returned value.</param>
    /// <returns><c>true</c> if could retrieve value, <c>false</c> otherwise.</returns>
    public bool GetValue(XRController controller, out Vector2 value)
    {
        return controller.inputDevice.TryGetFeatureValue(inputFeature, out value);
    }

    /// <summary>
    /// Invokes the OnValueChanged event.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">The value.</param>
    public void InvokeEvent(XRController controller, Vector2 value)
    {
        previousValue = value;
        OnValueChange?.Invoke(controller, value);
    }
}