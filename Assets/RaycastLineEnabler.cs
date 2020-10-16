using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RaycastLineEnabler : MonoBehaviour
{
    private XRRayInteractor interactor;
    private XRInteractorLineVisual visual;

    private bool isHitting = false;

    public bool IsHitting
    {
        get => isHitting;
        private set
        {
            if (isHitting == value) return;
            isHitting = value;
            ToggleInteractor(value);
        }
    }

    private void Awake()
    {
        interactor = gameObject.GetComponent<XRRayInteractor>();
        visual = gameObject.GetComponent<XRInteractorLineVisual>();
    }

    private void ToggleInteractor(bool value)
    {
        visual.enabled = value;
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
