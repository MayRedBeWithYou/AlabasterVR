using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/MockButton", fileName = "NewMockButtonHandler")]
public class MockButtonHandler : ButtonHandler
{
    public ButtonHandler mockedButton;
    public KeyCode key;

    public override void HandleState(XRController controller)
    {
        if (Input.GetKeyDown(key))
        {
            if (!previousPress)
            {
                previousPress = true;
                mockedButton.InvokeDownEvent(controller);
            }    
        }
        if (Input.GetKeyUp(key))
        {
            if (previousPress)
            {
                previousPress = false;
                mockedButton.InvokeUpEvent(controller);
            }
        }
    }
}