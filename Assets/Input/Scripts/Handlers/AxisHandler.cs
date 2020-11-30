using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/Axis", fileName = "NewAxisHandler")]
public class AxisHandler : InputHandler<float>, ISerializationCallbackReceiver
{
    public enum Axis
    {
        None,
        Trigger,
        Grip
    }

    public Axis axis = Axis.None;

    private InputFeatureUsage<float> inputFeature;
    public void OnAfterDeserialize()
    {
        inputFeature = new InputFeatureUsage<float>(axis.ToString());
    }

    public void OnBeforeSerialize() { }
    public override bool TryGetValue(XRController controller, out float value)
    {
        if (controller.inputDevice.TryGetFeatureValue(inputFeature, out value))
        {
            if (value < controller.axisToPressThreshold) value = 0f;
            return true;
        }
        return false;
    }
}
