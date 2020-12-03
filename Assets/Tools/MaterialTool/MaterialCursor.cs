using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCursor : MonoBehaviour
{
    private MeshCollider meshCollider;

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
    }

    public void GenerateCollider()
    {
        meshCollider.enabled = false;
        meshCollider.enabled = true;
    }

    private void Update()
    {
        
    }
}
