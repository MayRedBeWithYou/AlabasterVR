// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="OperationManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Class OperationManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class OperationManager : MonoBehaviour
{
    private static OperationManager _instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static OperationManager Instance => _instance;

    /// <summary>
    /// The undo button
    /// </summary>
    [Header("Buttons")]
    public ButtonHandler undoButton;
    /// <summary>
    /// The redo button
    /// </summary>
    public ButtonHandler redoButton;

    /// <summary>
    /// The undo audio
    /// </summary>
    [Header("AudioClips")]
    public AudioClip undoAudio;
    /// <summary>
    /// The redo audio
    /// </summary>
    public AudioClip redoAudio;

    /// <summary>
    /// The undo operations
    /// </summary>
    [Header("Stacks")]
    public Stack<IOperation> undoOperations = new Stack<IOperation>();

    /// <summary>
    /// The redo operations
    /// </summary>
    public Stack<IOperation> redoOperations = new Stack<IOperation>();

    /// <summary>
    /// Delegate StackChanged
    /// </summary>
    public delegate void StackChanged();

    /// <summary>
    /// Occurs when [on stacks changed].
    /// </summary>
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


    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        undoOperations.Clear();
        redoOperations.Clear();
    }

    /// <summary>
    /// Pushes the operation.
    /// </summary>
    /// <param name="op">The op.</param>
    public void PushOperation(IOperation op)
    {
        undoOperations.Push(op);
        redoOperations.Clear();
        GC.Collect();
        Debug.Log($"Added to stack: {op.GetType().Name}");
        OnStacksChanged?.Invoke();
    }

    /// <summary>
    /// Undoes this instance.
    /// </summary>
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
    /// <summary>
    /// Redoes this instance.
    /// </summary>
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

    /// <summary>
    /// Class StackPreview.
    /// Implements the <see cref="UnityEditor.Editor" />
    /// </summary>
    /// <seealso cref="UnityEditor.Editor" />
    [CustomEditor(typeof(OperationManager))]
    public class StackPreview : Editor
    {

        private void OnEnable()
        {
            var om = (OperationManager)target;
            om.OnStacksChanged += this.Repaint;
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var bold = new GUIStyle();
            bold.fontStyle = FontStyle.Bold;
            bold.normal.textColor = Color.white;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            GUILayout.Label("Undo", bold);
            var manager = (OperationManager)target;

            foreach (IOperation op in manager.undoOperations)
            {
                GUILayout.Label(op.GetType().Name);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Redo", bold);

            foreach (IOperation op in manager.redoOperations)
            {
                GUILayout.Label(op.GetType().Name);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
