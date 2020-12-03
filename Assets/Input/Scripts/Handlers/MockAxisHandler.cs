using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/MockAxis", fileName = "NewMockAxisHandler")]
public class MockAxisHandler : AxisHandler
{
    public AxisHandler mockedAxis;
    public KeyCode key;
    public float mockValue = 1f;

    public override void HandleState(XRController controller)
    {
        float value = previousValue;
        if (Input.GetKeyDown(key)) value = mockValue;
        if (Input.GetKeyUp(key)) value = 0f;
        if (previousValue != value) mockedAxis.InvokeEvent(controller, value);
        previousValue = value;
    }
}