using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/Axis2D", fileName = "NewAxisHandler2D")]
public class AxisHandler2D : InputHandler<Vector2>, ISerializationCallbackReceiver
{
    public enum Axis2D
    {
        None,
        Primary2DAxis,
        Secondary2DAxis
    }

    public Axis2D axis = Axis2D.None;

    private InputFeatureUsage<Vector2> inputFeature;

    public void OnAfterDeserialize()
    {
        inputFeature = new InputFeatureUsage<Vector2>(axis.ToString());
    }

    public void OnBeforeSerialize() { }

    public override bool TryGetValue(XRController controller, out Vector2 value)
    {
        return controller.inputDevice.TryGetFeatureValue(inputFeature, out value);
    }
}
