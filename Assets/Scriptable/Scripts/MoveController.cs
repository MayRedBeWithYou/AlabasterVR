using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveController : MonoBehaviour
{
    private static MoveController _instance;

    public static MoveController Instance => _instance;

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

    private Quaternion refRot;
    private Vector3 refControllers;
    private Vector3 refUp;
    private Vector3 refPos;
    private Vector3 refCenter;

    private Vector3 center => (leftController.transform.position + rightController.transform.position) / 2;

    [SerializeField]
    private GameObject grabbedObject;

    private Transform originalParent;

    [SerializeField]
    private IMovable movable;

    [SerializeField]
    private IResizable resizable;

    private TransformObject before;

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
            float scale = Vector3.Distance(leftController.transform.position, rightController.transform.position) / refDist;
            if (resizable != null)
            {
                resizable.SetScale(refScale * scale);
            }

            if (movable != null)
            {
                Vector3 displacement = (rightController.transform.position - leftController.transform.position).normalized;
                Vector3 controllersUp = (leftController.transform.up + rightController.transform.up).normalized;

                Quaternion quat = Quaternion.FromToRotation(refControllers, displacement) * Quaternion.FromToRotation(refUp, controllersUp);

                movable.SetPosition(quat * (refPos - refCenter) * scale + center);
                movable.SetRotation(quat * refRot);
            }
        }
        if(grab!=GrabState.None)LayerManager.Instance.DrawLayerBorder(LayerManager.Instance.ActiveLayer);
    }


    private void OnGrabHold(XRController controller)
    {
        switch (grab)
        {
            case GrabState.None:
                grab = GrabState.One;
                var hover = controller.GetComponent<OnHoverEventHandler>();
                if (hover.Current != null) grabbedObject = hover.Current;
                else
                {
                    grabbedObject = LayerManager.Instance.ActiveLayer.gameObject;
                    before = new TransformObject(grabbedObject.transform.position, grabbedObject.transform.localScale, grabbedObject.transform.rotation);
                }

                originalParent = grabbedObject.transform.parent;

                movable = grabbedObject.GetComponent<IMovable>();
                resizable = grabbedObject.GetComponent<IResizable>();

                if (movable != null) grabbedObject.transform.parent = controller.transform;
                break;
            case GrabState.One:
                grab = GrabState.Both;

                var pos = grabbedObject.transform.position;
                grabbedObject.transform.parent = originalParent;
                grabbedObject.transform.position = pos;

                refDist = Vector3.Distance(leftController.transform.position, rightController.transform.position);
                refPos = grabbedObject.transform.position;
                refCenter = center;
                refScale = grabbedObject.transform.localScale;
                refRot = grabbedObject.transform.rotation;
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
                if (controller.transform == grabbedObject.transform.parent)
                {
                    var pos = grabbedObject.transform.position;
                    grabbedObject.transform.parent = originalParent;
                    grabbedObject.transform.position = pos;
                }
                if (before != null)
                {
                    TransformObject after = new TransformObject(grabbedObject.transform);
                    LayerMoveOperation op = new LayerMoveOperation(LayerManager.Instance.ActiveLayer, before, after);
                    OperationManager.Instance.PushOperation(op);
                    before = null;
                }
                grabbedObject = null;
                originalParent = null;
                movable = null;
                resizable = null;
                
                break;
            case GrabState.Both:
                grab = GrabState.One;
                grabbedObject.transform.parent = controller == leftController ? rightController.transform : leftController.transform;
                ToolController.Instance.ToggleSelectedTool(true);
                break;
            default:
                break;
        }
    }
}
