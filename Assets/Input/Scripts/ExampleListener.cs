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
        rightPrimaryAxisHandler2D.StateChanged += PrintRightPrimaryAxis;
        rightTriggerHandler.StateChanged += PrintRightTrigger;
        rightPrimaryAxisClickHandler.StateChanged += PrintRightPrimaryAxisButtonDown;
        rightPrimaryAxisClickHandler.StateChanged += PrintRightPrimaryAxisButtonUp;
        rightPrimaryButtonHandler.StateChanged += PrintRightPrimaryButtonDown;
        rightPrimaryButtonHandler.StateChanged += PrintRightPrimaryButtonUp;
        rightSecondaryButtonHandler.StateChanged += PrintRightSecondaryButtonDown;
        rightSecondaryButtonHandler.StateChanged += PrintRightSecondaryButtonUp;

        leftPrimaryAxisHandler2D.StateChanged += PrintLeftPrimaryAxis;
        leftTriggerHandler.StateChanged += PrintLeftTrigger;
        leftPrimaryAxisClickHandler.StateChanged += PrintLeftPrimaryAxisButtonDown;
        leftPrimaryAxisClickHandler.StateChanged += PrintLeftPrimaryAxisButtonUp;
        leftPrimaryButtonHandler.StateChanged += PrintLeftPrimaryButtonDown;
        leftPrimaryButtonHandler.StateChanged += PrintLeftPrimaryButtonUp;
        leftSecondaryButtonHandler.StateChanged += PrintLeftSecondaryButtonDown;
        leftSecondaryButtonHandler.StateChanged += PrintLeftSecondaryButtonUp;
    }

    public void OnDisable()
    {
        rightPrimaryAxisHandler2D.StateChanged -= PrintRightPrimaryAxis;
        rightTriggerHandler.StateChanged -= PrintRightTrigger;
        rightPrimaryAxisClickHandler.StateChanged -= PrintRightPrimaryAxisButtonDown;
        rightPrimaryAxisClickHandler.StateChanged -= PrintRightPrimaryAxisButtonUp;
        rightPrimaryButtonHandler.StateChanged -= PrintRightPrimaryButtonDown;
        rightPrimaryButtonHandler.StateChanged -= PrintRightPrimaryButtonUp;
        rightSecondaryButtonHandler.StateChanged -= PrintRightSecondaryButtonDown;
        rightSecondaryButtonHandler.StateChanged += PrintRightSecondaryButtonUp;

        leftPrimaryAxisHandler2D.StateChanged -= PrintLeftPrimaryAxis;
        leftTriggerHandler.StateChanged -= PrintLeftTrigger;
        leftPrimaryAxisClickHandler.StateChanged -= PrintLeftPrimaryAxisButtonDown;
        leftPrimaryAxisClickHandler.StateChanged -= PrintLeftPrimaryAxisButtonUp;
        leftPrimaryButtonHandler.StateChanged -= PrintLeftPrimaryButtonDown;
        leftPrimaryButtonHandler.StateChanged -= PrintLeftPrimaryButtonUp;
        leftSecondaryButtonHandler.StateChanged -= PrintLeftSecondaryButtonDown;
        leftSecondaryButtonHandler.StateChanged -= PrintLeftSecondaryButtonUp;
    }

    private void PrintRightPrimaryAxisButtonDown(object sender, bool value)
    {
        Debug.Log("Right primary axis button down");
    }

    private void PrintRightPrimaryAxisButtonUp(object sender, bool value)
    {
        Debug.Log("Right primary axis button up");
    }
    private void PrintRightPrimaryButtonDown(object sender, bool value)
    {
        Debug.Log("Right primary button down");
    }

    private void PrintRightPrimaryButtonUp(object sender, bool value)
    {
        Debug.Log("Right primary button up");
    }
    private void PrintRightSecondaryButtonDown(object sender, bool value)
    {
        Debug.Log("Right secondary button down");
    }

    private void PrintRightSecondaryButtonUp(object sender, bool value)
    {
        Debug.Log("Right secondary button up");
    }

    private void PrintRightPrimaryAxis(object sender, Vector2 value)
    {
        Debug.Log($"Right primary axis - X: {value.x}, Y: {value.y}");
    }

    private void PrintRightTrigger(object sender, float value)
    {
        Debug.Log($"Right trigger value: {value}");
    }
    private void PrintLeftPrimaryAxisButtonDown(object sender, bool value)
    {
        Debug.Log("Left primary axis button down");
    }

    private void PrintLeftPrimaryAxisButtonUp(object sender, bool value)
    {
        Debug.Log("Left primary axis button up");
    }
    private void PrintLeftPrimaryButtonDown(object sender, bool value)
    {
        Debug.Log("Left primary button down");
    }

    private void PrintLeftPrimaryButtonUp(object sender, bool value)
    {
        Debug.Log("Left primary button up");
    }
    private void PrintLeftSecondaryButtonDown(object sender, bool value)
    {
        Debug.Log("Left secondary button down");
    }

    private void PrintLeftSecondaryButtonUp(object sender, bool value)
    {
        Debug.Log("Left secondary button up");
    }

    private void PrintLeftPrimaryAxis(object sender, Vector2 value)
    {
        Debug.Log($"Left primary axis - X: {value.x}, Y: {value.y}");
    }

    private void PrintLeftTrigger(object sender, float value)
    {
        Debug.Log($"Left trigger value: {value}");
    }
}
