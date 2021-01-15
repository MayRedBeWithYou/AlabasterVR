// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 10-09-2020
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="Readme.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using UnityEngine;

/// <summary>
/// Class Readme.
/// Implements the <see cref="UnityEngine.ScriptableObject" />
/// </summary>
/// <seealso cref="UnityEngine.ScriptableObject" />
public class Readme : ScriptableObject {
    /// <summary>
    /// The icon
    /// </summary>
    public Texture2D icon;
    /// <summary>
    /// The title
    /// </summary>
    public string title;
    /// <summary>
    /// The sections
    /// </summary>
    public Section[] sections;
    /// <summary>
    /// The loaded layout
    /// </summary>
    public bool loadedLayout;

    /// <summary>
    /// Class Section.
    /// </summary>
    [Serializable]
	public class Section {
        /// <summary>
        /// The heading
        /// </summary>
        public string heading, text, linkText, url;
	}
}
