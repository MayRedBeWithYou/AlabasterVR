// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-04-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-04-2021
// ***********************************************************************
// <copyright file="Movable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class Movable.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="IMovable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="IMovable" />
public class Movable : MonoBehaviour, IMovable
{
    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="pos">The position.</param>
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    /// <summary>
    /// Sets the rotation.
    /// </summary>
    /// <param name="rot">The rot.</param>
    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }
}

