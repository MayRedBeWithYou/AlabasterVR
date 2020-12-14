using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    public List<ButtonHandler> buttonHandlers = new List<ButtonHandler>();

    public List<AxisHandler> axisHandlers = new List<AxisHandler>();

    public List<AxisHandler2D> axisHandler2Ds = new List<AxisHandler2D>();
    
    public XRController controller;

    private OnHoverEventHandler onHoverHandler = null;

    private XRInteractorLineVisual visual = null;

    private void Awake()
    {
        onHoverHandler = GetComponent<OnHoverEventHandler>();
        visual = GetComponent<XRInteractorLineVisual>();

        onHoverHandler?.OnHoverEnter.AddListener((c) => visual.enabled = true);
        onHoverHandler?.OnHoverExit.AddListener((c) => visual.enabled = false);
    }

    private void Update()
    {
        HandleButtonEvents();
        HandleAxis2DEvents();
        HandleAxisEvents();
    }

    private void HandleButtonEvents()
    {
        foreach (ButtonHandler handler in buttonHandlers)
            handler.HandleState(controller);
    }

    private void HandleAxis2DEvents()
    {
        foreach (AxisHandler2D handler in axisHandler2Ds)
            handler.HandleState(controller);
    }

    public void HandleAxisEvents()
    {
        foreach (AxisHandler handler in axisHandlers)
            handler.HandleState(controller);
    }
}

