using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MaterialCursor : MonoBehaviour
{
    abstract public int AddMaterialKernel { get; }
    abstract public int RemoveMaterialKernel { get; }

    public abstract ComputeShader PrepareShader(Color color);
    
    public abstract void Enable();

    public abstract void Disable();

    public abstract void SetColor(Color color);

    public abstract void UpdateActiveChunks();
    public abstract void IncreaseSize();
    public abstract void DecreaseSize();

    public abstract void SetMaximumSize(float value);

    public abstract void SetMinimumSize(float value);

    public abstract void SetSizeToDefault();
}
