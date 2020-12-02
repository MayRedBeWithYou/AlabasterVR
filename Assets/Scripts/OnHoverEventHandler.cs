using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable]
public class OnHoverEnterEvent : UnityEvent<XRController> { }

[Serializable]
public class OnHoverExitEvent : UnityEvent<XRController> { }

public class OnHoverEventHandler : MonoBehaviour
{
    private XRRayInteractor interactor;
    private XRController controller;

    private bool isHitting = false;

    public OnHoverEnterEvent OnHoverEnter;
    public OnHoverExitEvent OnHoverExit;

    public bool IsHitting
    {
        get => isHitting;
        private set
        {
            if (isHitting != value)
            {
                if (value) OnHoverEnter?.Invoke(controller); 
                else OnHoverExit?.Invoke(controller);
                isHitting = value;
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
        bool isValid = false;
        int maxDistance = (int)interactor.maxRaycastDistance;
        Vector3 pos = interactor.attachTransform.position;
        Vector3 fwd = interactor.attachTransform.TransformDirection(Vector3.forward);
        if (interactor.TryGetHitInfo(ref pos, ref fwd, ref maxDistance, ref isValid))
        {
            IsHitting = true;
        }
        else IsHitting = false;
    }
}
