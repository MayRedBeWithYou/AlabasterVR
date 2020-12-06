using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResizeController : MonoBehaviour
{
    public enum GrabState
    {
        None,
        One,
        Both
    }

    [Header("Controllers")]
    public XRController leftController;
    public XRController rightController;

    [Header("Buttons")]
    public AxisHandler LeftGripHandler;
    public AxisHandler RightGripHandler;

    private float leftVal;
    private float rightVal;

    private GrabState grab = GrabState.None;

    private float refDist;

    private Vector3 center => (leftController.transform.position + rightController.transform.position) / 2;

    void Awake()
    {
        LeftGripHandler.OnValueChange += (controller, value) =>
        {
            if (value != leftVal)
            {
                if (value > 0.5f != leftVal > 0.5f)
                {
                    if (value > 0.5f) OnGrabHold(controller);
                    else OnGrabRelease(controller);
                }
                leftVal = value;
            }
        };

        RightGripHandler.OnValueChange += (controller, value) =>
        {
            if (value != rightVal)
            {
                if (value > 0.5f != rightVal > 0.5f)
                {
                    if (value > 0.5f) OnGrabHold(controller);
                    else OnGrabRelease(controller);
                }
                rightVal = value;
            }
        };
    }

    void Update()
    {
        if (grab == GrabState.Both)
        {
            Vector3 pos = center - LayerManager.Instance.ActiveLayer.transform.localScale * LayerManager.Instance.ActiveLayer.Size / 2;
            
            LayerManager.Instance.ActiveLayer.transform.position = pos;
            float dist = Vector3.Distance(leftController.transform.position, rightController.transform.position);
            LayerManager.Instance.ActiveLayer.transform.localScale = Vector3.one * dist / refDist;
        }
    }


    private void OnGrabHold(XRController controller)
    {
        switch (grab)
        {
            case GrabState.None:
                grab = GrabState.One;
                LayerManager.Instance.ActiveLayer.transform.parent = controller.transform;
                break;
            case GrabState.One:
                grab = GrabState.Both;
                LayerManager.Instance.ActiveLayer.transform.parent = LayerManager.Instance.LayersHolder.transform;
                refDist = Vector3.Distance(leftController.transform.position, rightController.transform.position);
                break;
            default:
                break;
        }
    }

    private void OnGrabRelease(XRController controller)
    {
        switch (grab)
        {
            case GrabState.One:
                grab = GrabState.None;
                LayerManager.Instance.ActiveLayer.transform.parent = LayerManager.Instance.LayersHolder.transform;
                break;
            case GrabState.Both:
                grab = GrabState.One;
                LayerManager.Instance.ActiveLayer.transform.parent = controller == leftController ? rightController.transform : leftController.transform;
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(center, 0.05f);
    }
}
