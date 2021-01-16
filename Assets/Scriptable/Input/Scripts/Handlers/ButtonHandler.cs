using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/Button", fileName = "NewButtonHandler")]
public class ButtonHandler : InputHandler
{
    public bool IsPressed => previousPress;

    public InputHelpers.Button button = InputHelpers.Button.None;

    public delegate void StateChange(XRController controller);

    public event StateChange OnButtonDown;
    public event StateChange OnButtonUp;

    protected bool previousPress = false;

    public override void HandleState(XRController controller)
    {
        if (controller.inputDevice.IsPressed(button, out bool isPressed, controller.axisToPressThreshold))
        {
            if (previousPress != isPressed)
            {
                previousPress = isPressed;

                if (isPressed) OnButtonDown?.Invoke(controller);
                else OnButtonUp?.Invoke(controller);
            }
        }
    }

    public void InvokeDownEvent(XRController controller)
    {
        previousPress = true;
        OnButtonDown?.Invoke(controller);
    }

    public void InvokeUpEvent(XRController controller)
    {
        previousPress = false;
        OnButtonUp?.Invoke(controller);
    }
}
