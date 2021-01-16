using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Input/MockAxis2D", fileName = "NewMockAxis2DHandler")]
public class MockAxis2DHandler : AxisHandler2D
{
    public AxisHandler2D mockedAxis2D;
    public KeyCode keyUp;
    public KeyCode keyDown;
    public KeyCode keyLeft;
    public KeyCode keyRight;
    public float mockValue = 1f;

    public override void HandleState(XRController controller)
    {
        Vector2 value = Vector2.zero;
        if (Input.GetKey(keyUp)) value.y += mockValue;
        if (Input.GetKey(keyDown)) value.y -= mockValue;
        if (Input.GetKey(keyLeft)) value.x -= mockValue;
        if (Input.GetKey(keyRight)) value.x += mockValue;
        if (previousValue != value) mockedAxis2D.InvokeEvent(controller, value);
        previousValue = value;
    }
}