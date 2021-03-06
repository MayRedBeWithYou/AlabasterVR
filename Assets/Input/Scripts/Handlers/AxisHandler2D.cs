﻿using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/Axis2D", fileName = "NewAxisHandler2D")]
public class AxisHandler2D : InputHandler, ISerializationCallbackReceiver
{
    public enum Axis2D
    {
        None,
        Primary2DAxis,
        Secondary2DAxis
    }

    public Vector2 Value => previousValue;

    public delegate void ValueChange(XRController controller, Vector2 value);
    public event ValueChange OnValueChange;

    public Axis2D axis = Axis2D.None;

    private InputFeatureUsage<Vector2> inputFeature;
    protected Vector2 previousValue = Vector2.zero;

    public void OnAfterDeserialize()
    {
        inputFeature = new InputFeatureUsage<Vector2>(axis.ToString());
    }

    public void OnBeforeSerialize() { }

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

    public bool GetValue(XRController controller, out Vector2 value)
    {
        return controller.inputDevice.TryGetFeatureValue(inputFeature, out value);
    }

    public void InvokeEvent(XRController controller, Vector2 value)
    {
        previousValue = value;
        OnValueChange?.Invoke(controller, value);
    }
}