using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private static LightManager _instance;

    public static LightManager Instance => _instance;

    [Header("References")]

    public GameObject LightHolder;

    public GameObject LightPrefab;

    public List<SceneLight> lights { get; private set; }


    public delegate void LightChanged(SceneLight light);

    public static event LightChanged LightAdded;

    public static event LightChanged LightRemoved;

    private int lightCount = 1;

    public float distance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        lights = new List<SceneLight>();

        SceneLight light = Instantiate(LightPrefab, LightHolder.transform).GetComponent<SceneLight>();
        light.name = $"Główne światło";

        light.transform.position = new Vector3(0, 3, 0);
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        light.light.intensity = 1;

        light.SetColor(light.light.color);
        lights.Add(light);
    }

    public SceneLight CreateLight()
    {
        SceneLight light = Instantiate(LightPrefab, LightHolder.transform).GetComponent<SceneLight>();
        light.name = $"Światło {lightCount++}";

        Vector3 lookDirection = Camera.main.transform.forward;

        light.transform.position = Camera.main.transform.position + lookDirection.normalized * distance;
        light.transform.rotation = Quaternion.LookRotation(-lookDirection, Vector3.up);

        light.SetColor(light.light.color);
        lights.Add(light);
        LightAdded?.Invoke(light);
        return light;
    }

    public void RemoveLight(SceneLight light)
    {
        lights.Remove(light);
        light.gameObject.SetActive(false);
        Destroy(light.gameObject);
        LightRemoved?.Invoke(light);
    }
}
