// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 12-03-2020
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="InputHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

/// <summary>
/// Class InputHandler.
/// Implements the <see cref="UnityEngine.ScriptableObject" />
/// </summary>
/// <seealso cref="UnityEngine.ScriptableObject" />
public abstract class InputHandler : ScriptableObject
{
    /// <summary>
    /// Handles the state.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public virtual void HandleState(XRController controller) { }
}