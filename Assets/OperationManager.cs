﻿using System.Collections;
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

    [Header("Stacks")]
    public Stack<IOperation> undoOperations = new Stack<IOperation>();

    public Stack<IOperation> redoOperations = new Stack<IOperation>();

    public delegate void StackChanged();

    public event StackChanged OnStacksChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        undoButton.OnButtonDown += (c) => Undo();
        redoButton.OnButtonDown += (c) => Redo();
    }

    public void Clear()
    {
        undoOperations.Clear();
        redoOperations.Clear();
    }

    public void PushOperation(IOperation op)
    {
        undoOperations.Push(op);
        redoOperations.Clear();
        Debug.Log($"Added to stack: {op.GetType().Name}");
        OnStacksChanged?.Invoke();
    }

    public void Undo()
    {
        if (undoOperations.Count == 0) return;
        IOperation op = undoOperations.Pop();
        op.Revert();
        redoOperations.Push(op);
        Debug.Log($"Undo: {op.GetType().Name}");
        OnStacksChanged?.Invoke();
    }
    public void Redo()
    {
        if (redoOperations.Count == 0) return;
        IOperation op = redoOperations.Pop();
        op.Apply();
        undoOperations.Push(op);
        Debug.Log($"Redo: {op.GetType().Name}");
        OnStacksChanged?.Invoke();
    }

    [CustomEditor(typeof(OperationManager))]
    public class StackPreview : Editor
    {

        private void OnEnable()
        {
            var om = (OperationManager)target;
            om.OnStacksChanged += this.Repaint;
        }

        public override void OnInspectorGUI()
        {
            var bold = new GUIStyle();
            bold.fontStyle = FontStyle.Bold;
            GUILayout.Label("Undo", bold);
            var manager = (OperationManager)target;

            foreach (IOperation op in manager.undoOperations)
            {
                GUILayout.Label(op.GetType().Name);
            }

            GUILayout.Label("Redo", bold);

            foreach (IOperation op in manager.redoOperations)
            {
                GUILayout.Label(op.GetType().Name);
            }
        }
    }
}
