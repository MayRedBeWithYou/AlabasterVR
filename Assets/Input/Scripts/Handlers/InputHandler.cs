using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public abstract class InputHandler<T> : ScriptableObject where T : IEquatable<T>
{
    public T Value => previousValue;
    protected T previousValue;
    public event EventHandler<T> StateChanged;
    public abstract bool TryGetValue(XRController controller, out T value);
    public virtual void HandleState(XRController controller)
    {
        if(TryGetValue(controller, out T value) && !previousValue.Equals(value))
        {
            InvokeEvent(value);
        }
    }
    public void InvokeEvent(T eventArg)
    {
        previousValue = eventArg;
        StateChanged?.Invoke(this, eventArg);
    }
}
