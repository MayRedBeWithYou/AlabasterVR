using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/Button", fileName = "NewButtonHandler")]
public class ButtonHandler : InputHandler<bool>
{
    public InputHelpers.Button button = InputHelpers.Button.None;

    public override bool TryGetValue(XRController controller, out bool value)
    {
        return controller.inputDevice.IsPressed(button, out value, controller.axisToPressThreshold);
    }
}