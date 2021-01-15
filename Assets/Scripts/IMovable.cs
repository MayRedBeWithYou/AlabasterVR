// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-04-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-04-2021
// ***********************************************************************
// <copyright file="IMovable.cs" company="">
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
/// Interface IMovable
/// </summary>
public interface IMovable
{
    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="pos">The position.</param>
    void SetPosition(Vector3 pos);
    /// <summary>
    /// Sets the rotation.
    /// </summary>
    /// <param name="rot">The rot.</param>
    void SetRotation(Quaternion rot);
}

