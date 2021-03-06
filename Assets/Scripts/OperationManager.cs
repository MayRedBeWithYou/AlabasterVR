﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OperationManager : MonoBehaviour
{
    private static OperationManager _instance;

    public static OperationManager Instance => _instance;

    [Header("Buttons")]
    public ButtonHandler undoButton;
    public ButtonHandler redoButton;

    [Header("AudioClips")]
    public AudioClip undoAudio;
    public AudioClip redoAudio;

    [Header("Stacks")]
    public Stack<IOperation> undoOperations = new Stack<IOperation>();

    public Stack<IOperation> redoOperations = new Stack<IOperation>();

    public delegate void StackChanged();

    public event StackChanged OnStacksChanged;

    private AudioSource source;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        source = GetComponent<AudioSource>();

        undoButton.OnButtonDown += (c) => Undo();
        redoButton.OnButtonDown += (c) => Redo();
    }


    public void Clear()
    {
        undoOperations.Clear();
        redoOperations.Clear();
        GC.Collect();
    }

    public void PushOperation(IOperation op)
    {
        undoOperations.Push(op);
        redoOperations.Clear();
        GC.Collect();
        Debug.Log($"Added to stack: {op.GetType().Name}");
        OnStacksChanged?.Invoke();
    }

    public void Undo()
    {
        if (undoOperations.Count == 0) return;
        IOperation op = undoOperations.Pop();
        op.Revert();
        redoOperations.Push(op);
        source.PlayOneShot(undoAudio);
        Debug.Log($"Undo: {op.GetType().Name}");
        OnStacksChanged?.Invoke();
    }
    public void Redo()
    {
        if (redoOperations.Count == 0) return;
        IOperation op = redoOperations.Pop();
        op.Apply();
        undoOperations.Push(op);
        source.PlayOneShot(redoAudio);
        Debug.Log($"Redo: {op.GetType().Name}");
        OnStacksChanged?.Invoke();
    }    
}
