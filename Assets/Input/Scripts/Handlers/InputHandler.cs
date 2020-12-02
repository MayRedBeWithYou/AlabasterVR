using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public abstract class InputHandler : ScriptableObject
{
    public virtual void HandleState(XRController controller) { }
}