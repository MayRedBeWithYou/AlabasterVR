using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    private static MeshGenerator _instance;

    public static MeshGenerator Instance => _instance;

    public int threadGroupSize = 8;

    public ComputeShader shader;

    public bool displayDebug;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    public void UpdateAllActiveChunks()
    {
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
            UpdateChunkMesh(chunk);
    }

    public void UpdateChunkMesh(Chunk chunk)
    {
        chunk.gpuMesh.DrawMesh(chunk.offset);
    }
}
