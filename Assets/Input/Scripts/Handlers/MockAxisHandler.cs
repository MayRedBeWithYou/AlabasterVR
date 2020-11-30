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
        if (Input.GetKeyDown(key)) mockedAxis.InvokeEvent(mockValue);
        if (Input.GetKeyUp(key)) mockedAxis.InvokeEvent(0f);
    }
}