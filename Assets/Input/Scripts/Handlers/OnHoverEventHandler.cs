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
public class OnHoverEnterEvent : UnityEvent<XRController, GameObject> { }

[Serializable]
public class OnHoverExitEvent : UnityEvent<XRController, GameObject> { }

public class OnHoverEventHandler : MonoBehaviour
{
    private XRRayInteractor interactor;
    private XRController controller;

    public OnHoverEnterEvent OnHoverEnter;
    public OnHoverExitEvent OnHoverExit;

    private XRInteractorLineVisual visual = null;

    [SerializeField]
    private GameObject current;

    public GameObject Current
    {
        get => current;
        set
        {
            if (current == null && !ReferenceEquals(current, null))
            {
                OnHoverExit?.Invoke(controller, current);
                current = null;
                ToggleInteractor(false);
            }
            if (value != current)
            {
                if (value is null)
                {
                    OnHoverExit?.Invoke(controller, current);
                    current = null;
                    ToggleInteractor(false);
                }
                else
                {
                    if (current != null)
                    {
                        OnHoverExit?.Invoke(controller, current);
                        ToggleInteractor(false);
                    }
                    current = value;
                    OnHoverEnter?.Invoke(controller, current);
                    ToggleInteractor(true);
                }
            }
        }
    }


    private void Awake()
    {
        interactor = GetComponent<XRRayInteractor>();
        visual = GetComponent<XRInteractorLineVisual>();
        controller = GetComponent<XRController>();
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

        if (interactor.GetCurrentRaycastHit(out RaycastHit hit))
        {
            Current = hit.collider.transform.parent.gameObject;
            visual.lineLength = hit.distance;
        }

        else if (interactor.TryGetHitInfo(ref a, ref b, ref c, ref d))
        {
            TrackedDeviceModel model;
            if (interactor.TryGetUIModel(out model))
            {
                ImplementationData data = (ImplementationData)model.GetType().GetProperty("implementationData", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(model, null);
                RaycastResult result = data.lastFrameRaycast;
                visual.lineLength = result.distance;

                var canvas = result.gameObject.GetComponentInParent<Canvas>();
                if (canvas != null) Current = canvas.gameObject;
                else Current = result.gameObject;
            }
        }
        else
        {
            Current = null;
        }
    }

    public void ToggleInteractor(bool value)
    {
        visual.enabled = value;
    }
}
