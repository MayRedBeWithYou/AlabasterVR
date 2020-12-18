using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockInputManager : MonoBehaviour
{
    public GameObject LeftController;
    public GameObject RightController;
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
            ToolController.Instance.SelectedTool = ToolController.Instance.Tools[0];
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            ToolController.Instance.SelectedTool = ToolController.Instance.Tools[1];
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            ToolController.Instance.SelectedTool = ToolController.Instance.Tools[5];
        }

    }
}