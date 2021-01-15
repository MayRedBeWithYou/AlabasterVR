// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-14-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-14-2021
// ***********************************************************************
// <copyright file="MockInputManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class MockInputManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class MockInputManager : MonoBehaviour
{
    /// <summary>
    /// The left controller
    /// </summary>
    public GameObject LeftController;
    /// <summary>
    /// The right controller
    /// </summary>
    public GameObject RightController;
    /// <summary>
    /// The camera
    /// </summary>
    public GameObject Camera;

    private CameraController[] CameraControllers;
    private void Awake()
    {
        CameraControllers = GetComponents<CameraController>();
        ActivateCamera();
    }

    void ActivateCamera()
    {
        foreach (var controller in CameraControllers)
        {
            controller.enabled = controller.targetCamera == Camera ? true : false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (var controller in CameraControllers)
            {
                controller.enabled = controller.targetCamera == LeftController ? true : false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (var controller in CameraControllers)
            {
                controller.enabled = controller.targetCamera == RightController ? true : false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (var controller in CameraControllers)
            {
                controller.enabled = controller.targetCamera == Camera ? true : false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateCamera();
            foreach (var controller in CameraControllers)
            {
                if(controller.targetCamera != Camera)
                controller.targetCamera.transform.position = Camera.transform.position;
                controller.targetCamera.transform.rotation = Camera.transform.rotation;
                controller.targetCamera.transform.position += Camera.transform.forward * 0.5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            ToolController.Instance.SelectedTool = ToolController.Instance.Tools.Find(tool => tool.toolName == "Material");
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            ToolController.Instance.SelectedTool = ToolController.Instance.Tools.Find(tool => tool.toolName == "Smooth"); ;
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            ToolController.Instance.SelectedTool = ToolController.Instance.Tools.Find(tool => tool.toolName == "Move");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ToolController.Instance.SelectedTool = ToolController.Instance.Tools.Find(tool => tool.toolName == "Paint");
        }
    }
}