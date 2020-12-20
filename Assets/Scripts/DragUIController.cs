using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DragUIController : MonoBehaviour
{
    private OnHoverEventHandler _hover;

    private Canvas currentUI;
    private XRController _controller;

    public AxisHandler grip;


    [SerializeField]
    private float gripVal;

    private bool isHolding => gripVal > 0.5f;


    public void Awake()
    {
        _hover = GetComponent<OnHoverEventHandler>();
        _controller = GetComponent<XRController>();
        _hover.OnHoverEnter.AddListener((c, ui) => { if(ui.tag == "DragUI") currentUI = ui; });
        _hover.OnHoverExit.AddListener((c, ui) => { if (ui.tag == "DragUI") currentUI = null; });

        grip.OnValueChange += HandleGripValueChange;
    }

    private void HandleGripValueChange(XRController controller, float value)
    {
        if (value != gripVal)
        {
            if (value > 0.5f != gripVal > 0.5f)
            {
                if (value > 0.5f) OnGrabHold();
                else OnGrabRelease();
            }
            gripVal = value;
        }
    }

    private void OnGrabHold()
    {
        if (currentUI != null)
        {
            currentUI.transform.parent = _controller.transform;
        }
    }

    private void OnGrabRelease()
    {
        if (currentUI != null)
        {
            var pos = currentUI.transform.position;
            currentUI.transform.parent = null;
            currentUI.transform.position = pos;
        }
    }
}
