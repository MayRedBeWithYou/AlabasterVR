using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCursor : MonoBehaviour
{
    private Material _material;
    [SerializeField]
    private float alpha = 0.1f;
    public Color Color
    {
        get => _material.color;
        set
        {
            value.a = alpha;
            _material.color = value;
        }
    }
    public abstract float Size { get; protected set; }
    private float _maximalSize;
    private float _minimalSize;

    public float MaximalSize
    {
        get => _maximalSize;
        set
        {
            if (Size > value) Size = value;
            _maximalSize = value;
        }
    }
    public float MinimalSize
    {
        get => _minimalSize;
        set
        {
            if (Size < value) Size = value;
            _minimalSize = value;
        }
    }
    public void SetSizeToDefault()
    {
        Size = (MaximalSize - MinimalSize) * 0.5f;
    }
    public float SizeChangeSpeed = 0.001f;

    public void DecreaseSize()
    {
        Size -= SizeChangeSpeed;
        if (Size < MinimalSize) Size = MinimalSize;
    }

    public void IncreaseSize()
    {
        Size += SizeChangeSpeed;
        if (Size > MaximalSize) Size = MaximalSize;
    }

    public abstract void UpdateActiveChunks();
    protected virtual void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }
}
