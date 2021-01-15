// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-07-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-07-2021
// ***********************************************************************
// <copyright file="IOperation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface IOperation
/// </summary>
public interface IOperation
{
    /// <summary>
    /// Applies the operation.
    /// </summary>
    void Apply();

    /// <summary>
    /// Reverts the operation.
    /// </summary>
    void Revert();
}
