using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLight : MonoBehaviour, IMovable
{
    public Light light { get; private set; }

    public string name;

    private MeshRenderer meshRenderer;

    public bool Enabled => light.enabled;

    public void Awake()
    {
        light = GetComponent<Light>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void ToggleLight(bool value)
    {
        if(value)
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
