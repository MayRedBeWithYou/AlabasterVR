using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/MockButton", fileName = "NewMockButtonHandler")]
public class MockButtonHandler : ButtonHandler
{
    public ButtonHandler mockedButton;
    public KeyCode key;

    public override void HandleState(XRController controller)
    {
        if(Input.GetKeyDown(key)) mockedButton.InvokeEvent(true);
        if(Input.GetKeyUp(key)) mockedButton.InvokeEvent(false);
    }
}