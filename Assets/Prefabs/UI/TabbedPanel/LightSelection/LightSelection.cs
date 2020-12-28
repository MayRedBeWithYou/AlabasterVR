using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSelection : MonoBehaviour
{
    public GameObject contentHolder;

    public GameObject lightItemPrefab;

    public Dictionary<SceneLight, LightItem> lights;

    public void Start()
    {
        lights = new Dictionary<SceneLight, LightItem>();

        foreach (SceneLight light in LightManager.Instance.lights)
        {
            LightAdded(light);
        }

        LightManager.LightAdded += LightAdded;
        LightManager.LightRemoved += LightRemoved;
    }

    private void LightRemoved(SceneLight layer)
    {
        LightItem item = lights[layer];
        lights.Remove(layer);
        item.gameObject.SetActive(false);
        Destroy(item.gameObject);
    }

    private void LightAdded(SceneLight light)
    {
        GameObject go = Instantiate(lightItemPrefab, contentHolder.transform);
        go.name = light.name;
        LightItem item = go.GetComponent<LightItem>();
        item.Light = light;
        lights.Add(light, item);
    }

    public void OnDestroy()
    {
        LightManager.LightAdded -= LightAdded;
        LightManager.LightRemoved -= LightRemoved;
    }
}
