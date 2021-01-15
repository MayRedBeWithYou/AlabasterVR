// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="AxisHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class AxisHandler.
/// Implements the <see cref="InputHandler" />
/// Implements the <see cref="UnityEngine.ISerializationCallbackReceiver" />
/// </summary>
/// <seealso cref="InputHandler" />
/// <seealso cref="UnityEngine.ISerializationCallbackReceiver" />
[CreateAssetMenu(menuName = "Input/Axis", fileName = "NewAxisHandler")]
public class AxisHandler : InputHandler, ISerializationCallbackReceiver
{
    public enum Axis
    {
        None,
        Trigger,
        Grip
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public float Value => previousValue;

    /// <summary>
    /// Delegate ValueChange
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">The value.</param>
    public delegate void ValueChange(XRController controller, float value);
    /// <summary>
    /// Occurs when Value changes.
    /// </summary>
    public event ValueChange OnValueChange;

    /// <summary>
    /// The axis handled.
    /// </summary>
    public Axis axis = Axis.None;

    private InputFeatureUsage<float> inputFeature;
    protected float previousValue = 0f;

    /// <summary>
    /// Implement this method to receive a callback after Unity deserializes your object.
    /// </summary>
    public void OnAfterDeserialize()
    {
        inputFeature = new InputFeatureUsage<float>(axis.ToString());
    }

    /// <summary>
    /// Implement this method to receive a callback before Unity serializes your object.
    /// </summary>
    public void OnBeforeSerialize() { }

    /// <summary>
    /// Handles the state of the axis.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public override void HandleState(XRController controller)
    {
        if(GetValue(controller, out float value))
        {
            if (value != previousValue)
            {
                previousValue = value;
                OnValueChange?.Invoke(controller, value);
            }
        }
        
    }

    /// <summary>
    /// Gets the value corresponding to axis position.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">Returned value.</param>
    /// <returns><c>true</c> if could retrieve value, <c>false</c> otherwise.</returns>
    public bool GetValue(XRController controller, out float value)
    {
        if (controller.inputDevice.TryGetFeatureValue(inputFeature, out value))
        {
            if (value < controller.axisToPressThreshold) value = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Invokes the OnValueChanged event.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">The value.</param>
    public void InvokeEvent(XRController controller, float value)
    {
        previousValue = value;
        OnValueChange?.Invoke(controller, value);
    }
}