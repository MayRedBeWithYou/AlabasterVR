using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneLight : MonoBehaviour, IMovable
{
    public Light light { get; private set; }

    private MeshRenderer meshRenderer;
    private float range;
    private float angle;

    public float Range
    {
        get => range;
        set
        {
            range = value;
            light.range = value;
        }
    }

    public float Angle
    {
        get => angle;
        set
        {
            angle = value;
            light.spotAngle = value;
        }
    }

    public bool Enabled => light.enabled;

    public void Awake()
    {
        light = GetComponent<Light>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        range = light.range;
        angle = light.spotAngle;
    }

    public void ToggleLight(bool value)
    {
        if (value)
        {
            light.enabled = true;
            meshRenderer.materials[1].color = light.color;
            meshRenderer.materials[1].SetColor("_EmissionColor", light.color);
        }
        else
        {
            light.enabled = false;
            meshRenderer.materials[1].color = Color.black;
            meshRenderer.materials[1].SetColor("_EmissionColor", Color.black);
        }
    }

    public void SetColor(Color color)
    {
        light.color = color;
        meshRenderer.materials[1].color = color;
        meshRenderer.materials[1].SetColor("_EmissionColor", color);
    }

    public void SetPosition(Vector3 pos)
    {
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }
}
