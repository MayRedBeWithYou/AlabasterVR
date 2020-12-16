﻿using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResizeController : MonoBehaviour
{
    private static ResizeController _instance;

    public static ResizeController Instance => _instance;

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

    private Vector3 refScale;
    private float refDist;

    private int handsOnUI;

    private Quaternion refRot;
    private Vector3 refControllers;
    private Vector3 refUp;
    private Vector3 refPos;
    private Vector3 refCenter;

    private Vector3 center => (leftController.transform.position + rightController.transform.position) / 2;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        OnHoverEventHandler leftHover = leftController.GetComponent<OnHoverEventHandler>();
        OnHoverEventHandler rightHover = rightController.GetComponent<OnHoverEventHandler>();

        leftHover.OnHoverEnter.AddListener((c, ui) => { if (leftVal > 0.5f) OnGrabRelease(c); handsOnUI++; });
        leftHover.OnHoverExit.AddListener((c, ui) => { handsOnUI--; });
        rightHover.OnHoverEnter.AddListener((c, ui) => { if (rightVal > 0.5f) OnGrabRelease(c); handsOnUI++; });
        rightHover.OnHoverExit.AddListener((c, ui) => { handsOnUI--; });

        LeftGripHandler.OnValueChange += (controller, value) =>
        {
            if (handsOnUI > 0) return;
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
            if (handsOnUI > 0) return;
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
        if (grab == GrabState.Both && handsOnUI == 0)
        {
            Layer layer = LayerManager.Instance.ActiveLayer;
            float scale = Vector3.Distance(leftController.transform.position, rightController.transform.position) / refDist;

            layer.transform.localScale = refScale * scale;
            Vector3 displacement = (rightController.transform.position - leftController.transform.position).normalized;
            Vector3 controllersUp = (leftController.transform.up + rightController.transform.up).normalized;

            Quaternion quat = Quaternion.FromToRotation(refControllers, displacement) * Quaternion.FromToRotation(refUp, controllersUp);

            layer.transform.position = quat * (refPos - refCenter) * scale + center;
            layer.transform.rotation = quat * refRot;
        }
    }


    private void OnGrabHold(XRController controller)
    {
        Layer layer = LayerManager.Instance.ActiveLayer;
        switch (grab)
        {
            case GrabState.None:
                grab = GrabState.One;
                layer.transform.parent = controller.transform;
                break;
            case GrabState.One:
                grab = GrabState.Both;
                layer.transform.parent = LayerManager.Instance.LayersHolder.transform;
                refDist = Vector3.Distance(leftController.transform.position, rightController.transform.position);
                refPos = layer.transform.position;
                refCenter = center;
                refScale = layer.transform.localScale;
                refRot = layer.transform.rotation;
                refControllers = (rightController.transform.position - leftController.transform.position).normalized;
                refUp = (leftController.transform.up + rightController.transform.up).normalized;
                ToolController.Instance.ToggleSelectedTool(false);
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
                if (controller.transform == LayerManager.Instance.ActiveLayer.transform.parent)
                    LayerManager.Instance.ActiveLayer.transform.parent = LayerManager.Instance.LayersHolder.transform;
                break;
            case GrabState.Both:
                grab = GrabState.One;
                LayerManager.Instance.ActiveLayer.transform.parent = controller == leftController ? rightController.transform : leftController.transform;

                ToolController.Instance.ToggleSelectedTool(true);
                break;
            default:
                break;
        }
    }
}
