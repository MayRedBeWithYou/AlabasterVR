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
    public ButtonHandler LeftGripHandler;
    public ButtonHandler RightGripHandler;

    private GrabState grab = GrabState.None;

    private float refDist;

    void Awake()
    {
        LeftGripHandler.OnButtonDown += OnGrabHold;
        LeftGripHandler.OnButtonUp += OnGrabRelease;
        
        RightGripHandler.OnButtonDown += OnGrabHold;
        RightGripHandler.OnButtonUp += OnGrabRelease;
    }

    void Update()
    {
        if (grab == GrabState.Both)
        {
            Vector3 center = leftController.transform.position + rightController.transform.position;
            center = leftController.transform.position + center / 2;

            LayerManager.Instance.activeLayer.transform.position = center;
            float dist = Vector3.Distance(leftController.transform.position, rightController.transform.position);
            LayerManager.Instance.activeLayer.transform.localScale = Vector3.one * dist / refDist;
        }
    }


    private void OnGrabHold(XRController controller)
    {
        switch (grab)
        {
            case GrabState.None:
                grab = GrabState.One;
                LayerManager.Instance.activeLayer.transform.parent = controller.transform;
                break;
            case GrabState.One:
                grab = GrabState.Both;
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
                LayerManager.Instance.activeLayer.transform.parent = LayerManager.Instance.LayersHolder.transform;
                break;
            case GrabState.Both:
                grab = GrabState.One;
                LayerManager.Instance.activeLayer.transform.parent = controller.transform;
                break;
            default:
                break;
        }
    }
}
