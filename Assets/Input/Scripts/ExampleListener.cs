using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExampleListener : MonoBehaviour
{
    [Header("Right controller")]
    public AxisHandler2D rightPrimaryAxisHandler2D = null;
    public AxisHandler rightTriggerHandler = null;
    public ButtonHandler rightPrimaryAxisClickHandler = null;
    public ButtonHandler rightPrimaryButtonHandler = null;
    public ButtonHandler rightSecondaryButtonHandler = null;

    [Header("Left controller")]
    public AxisHandler2D leftPrimaryAxisHandler2D = null;
    public AxisHandler leftTriggerHandler = null;
    public ButtonHandler leftPrimaryAxisClickHandler = null;
    public ButtonHandler leftPrimaryButtonHandler = null;
    public ButtonHandler leftSecondaryButtonHandler = null;

    public void OnEnable()
    {
        rightPrimaryAxisHandler2D.OnValueChange += PrintRightPrimaryAxis;
        rightTriggerHandler.OnValueChange += PrintRightTrigger;
        rightPrimaryAxisClickHandler.OnButtonDown += PrintRightPrimaryAxisButtonDown;
        rightPrimaryAxisClickHandler.OnButtonUp += PrintRightPrimaryAxisButtonUp;
        rightPrimaryButtonHandler.OnButtonDown += PrintRightPrimaryButtonDown;
        rightPrimaryButtonHandler.OnButtonUp += PrintRightPrimaryButtonUp;
        rightSecondaryButtonHandler.OnButtonDown += PrintRightSecondaryButtonDown;
        rightSecondaryButtonHandler.OnButtonUp += PrintRightSecondaryButtonUp;

        leftPrimaryAxisHandler2D.OnValueChange += PrintLeftPrimaryAxis;
        leftTriggerHandler.OnValueChange += PrintLeftTrigger;
        leftPrimaryAxisClickHandler.OnButtonDown += PrintLeftPrimaryAxisButtonDown;
        leftPrimaryAxisClickHandler.OnButtonUp += PrintLeftPrimaryAxisButtonUp;
        leftPrimaryButtonHandler.OnButtonDown += PrintLeftPrimaryButtonDown;
        leftPrimaryButtonHandler.OnButtonUp += PrintLeftPrimaryButtonUp;
        leftSecondaryButtonHandler.OnButtonDown += PrintLeftSecondaryButtonDown;
        leftSecondaryButtonHandler.OnButtonUp += PrintLeftSecondaryButtonUp;
    }

    public void OnDisable()
    {
        rightPrimaryAxisHandler2D.OnValueChange -= PrintRightPrimaryAxis;
        rightTriggerHandler.OnValueChange -= PrintRightTrigger;
        rightPrimaryAxisClickHandler.OnButtonDown -= PrintRightPrimaryAxisButtonDown;
        rightPrimaryAxisClickHandler.OnButtonUp -= PrintRightPrimaryAxisButtonUp;
        rightPrimaryButtonHandler.OnButtonDown -= PrintRightPrimaryButtonDown;
        rightPrimaryButtonHandler.OnButtonUp -= PrintRightPrimaryButtonUp;
        rightSecondaryButtonHandler.OnButtonDown -= PrintRightSecondaryButtonDown;
        rightSecondaryButtonHandler.OnButtonUp += PrintRightSecondaryButtonUp;

        leftPrimaryAxisHandler2D.OnValueChange -= PrintLeftPrimaryAxis;
        leftTriggerHandler.OnValueChange -= PrintLeftTrigger;
        leftPrimaryAxisClickHandler.OnButtonDown -= PrintLeftPrimaryAxisButtonDown;
        leftPrimaryAxisClickHandler.OnButtonUp -= PrintLeftPrimaryAxisButtonUp;
        leftPrimaryButtonHandler.OnButtonDown -= PrintLeftPrimaryButtonDown;
        leftPrimaryButtonHandler.OnButtonUp -= PrintLeftPrimaryButtonUp;
        leftSecondaryButtonHandler.OnButtonDown -= PrintLeftSecondaryButtonDown;
        leftSecondaryButtonHandler.OnButtonUp += PrintLeftSecondaryButtonUp;
    }

    private void PrintRightPrimaryAxisButtonDown(XRController controller)
    {
        Debug.Log("Right primary axis button down");
    }

    private void PrintRightPrimaryAxisButtonUp(XRController controller)
    {
        Debug.Log("Right primary axis button up");
    }
    private void PrintRightPrimaryButtonDown(XRController controller)
    {
        Debug.Log("Right primary button down");
    }

    private void PrintRightPrimaryButtonUp(XRController controller)
    {
        Debug.Log("Right primary button up");
    }
    private void PrintRightSecondaryButtonDown(XRController controller)
    {
        Debug.Log("Right secondary button down");
    }

    private void PrintRightSecondaryButtonUp(XRController controller)
    {
        Debug.Log("Right secondary button up");
    }

    private void PrintRightPrimaryAxis(XRController controller, Vector2 value)
    {
        Debug.Log($"Right primary axis - X: {value.x}, Y: {value.y}");
    }

    private void PrintRightTrigger(XRController controller, float value)
    {
        Debug.Log($"Right trigger value: {value}");
    }
    private void PrintLeftPrimaryAxisButtonDown(XRController controller)
    {
        Debug.Log("Left primary axis button down");
    }

    private void PrintLeftPrimaryAxisButtonUp(XRController controller)
    {
        Debug.Log("Left primary axis button up");
    }
    private void PrintLeftPrimaryButtonDown(XRController controller)
    {
        Debug.Log("Left primary button down");
    }

    private void PrintLeftPrimaryButtonUp(XRController controller)
    {
        Debug.Log("Left primary button up");
    }
    private void PrintLeftSecondaryButtonDown(XRController controller)
    {
        Debug.Log("Left secondary button down");
    }

    private void PrintLeftSecondaryButtonUp(XRController controller)
    {
        Debug.Log("Left secondary button up");
    }

    private void PrintLeftPrimaryAxis(XRController controller, Vector2 value)
    {
        Debug.Log($"Left primary axis - X: {value.x}, Y: {value.y}");
    }

    private void PrintLeftTrigger(XRController controller, float value)
    {
        Debug.Log($"Left trigger value: {value}");
    }
}
