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
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The trigger
        /// </summary>
        Trigger,
        /// <summary>
        /// The grip
        /// </summary>
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
    /// Occurs when [on value change].
    /// </summary>
    public event ValueChange OnValueChange;

    /// <summary>
    /// The axis
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
    /// Handles the state.
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
    /// Gets the value.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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
    /// Invokes the event.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="value">The value.</param>
    public void InvokeEvent(XRController controller, float value)
    {
        previousValue = value;
        OnValueChange?.Invoke(controller, value);
    }
}