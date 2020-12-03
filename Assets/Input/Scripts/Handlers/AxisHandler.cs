using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/Axis", fileName = "NewAxisHandler")]
public class AxisHandler : InputHandler, ISerializationCallbackReceiver
{
    public enum Axis
    {
        None,
        Trigger,
        Grip
    }

    public float Value => previousValue;

    public delegate void ValueChange(XRController controller, float value);
    public event ValueChange OnValueChange;

    public Axis axis = Axis.None;

    private InputFeatureUsage<float> inputFeature;
    protected float previousValue = 0f;

    public void OnAfterDeserialize()
    {
        inputFeature = new InputFeatureUsage<float>(axis.ToString());
    }

    public void OnBeforeSerialize() { }

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

    public bool GetValue(XRController controller, out float value)
    {
        if (controller.inputDevice.TryGetFeatureValue(inputFeature, out value))
        {
            if (value < controller.axisToPressThreshold) value = 0;
            return true;
        }
        return false;
    }

    public void InvokeEvent(XRController controller, float value)
    {
        previousValue = value;
        OnValueChange?.Invoke(controller, value);
    }
}