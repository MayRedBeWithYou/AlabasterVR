using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using static UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceModel;

[Serializable]
public class OnHoverEnterEvent : UnityEvent<XRController, Canvas> { }

[Serializable]
public class OnHoverExitEvent : UnityEvent<XRController, Canvas> { }

public class OnHoverEventHandler : MonoBehaviour
{
    private XRRayInteractor interactor;
    private XRController controller;

    public OnHoverEnterEvent OnHoverEnter;
    public OnHoverExitEvent OnHoverExit;

    private Canvas currentUI;

    public Canvas CurrentUI
    {
        get => currentUI;
        set
        {
            if (currentUI == null && !ReferenceEquals(currentUI, null))
            {
                OnHoverExit?.Invoke(controller, currentUI);
                currentUI = null;
            }
            if (value != currentUI)
            {
                if (value is null)
                {
                    OnHoverExit?.Invoke(controller, currentUI);
                    currentUI = null;
                }
                else
                {
                    if (currentUI != null)
                    {
                        OnHoverExit?.Invoke(controller, currentUI);
                    }
                    currentUI = value;
                    OnHoverEnter?.Invoke(controller, currentUI);
                }
            }
        }
    }


    private void Awake()
    {
        interactor = gameObject.GetComponent<XRRayInteractor>();
        controller = gameObject.GetComponent<XRController>();
    }

    void Update()
    {
        int maxDistance = (int)interactor.maxRaycastDistance;
        Vector3 pos = interactor.attachTransform.position;
        Vector3 fwd = interactor.attachTransform.forward;

        Vector3 a = Vector3.zero;
        Vector3 b = Vector3.zero;
        int c = 0;
        bool d = false;
        if (interactor.TryGetHitInfo(ref a, ref b, ref c, ref d))
        {
            TrackedDeviceModel model;
            if (interactor.TryGetUIModel(out model))
            {
                ImplementationData data = (ImplementationData)model.GetType().GetProperty("implementationData", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(model, null);
                RaycastResult result = data.lastFrameRaycast;

                CurrentUI = result.gameObject.GetComponentInParent<Canvas>();
            }
        }
        else
        {
            CurrentUI = null;
        }
    }
}
