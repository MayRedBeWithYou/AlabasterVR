using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    private static LayerManager _instance;

    public static LayerManager Instance => _instance;

    [Header("References")]

    public GameObject LayersHolder;

    private List<Layer> layers;

    public GameObject chunkPrefab;

    [Header("Parameters")]
    public int chunksInAxis;

    public int chunkSize;

    public float boundSize;

    [Header("Gizmos")]

    public bool showBoundGizmo;

    [Header("Debug info")]

    [SerializeField]
    private Layer _activeLayer;

    public List<Chunk> activeChunks;

    public Layer activeLayer
    {
        get => _activeLayer;
        set
        {
            activeChunks.Clear();
            _activeLayer = value;
        }
    }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        layers = new List<Layer>();
        activeChunks = new List<Chunk>();

        Chunk chunk = chunkPrefab.GetComponent<Chunk>();
        chunk.boundSize = boundSize;
        chunk.size = chunkSize;
        _activeLayer = AddNewLayer();
    }

    Layer AddNewLayer()
    {
        GameObject layerObject = new GameObject($"Layer {layers.Count + 1}");
        layerObject.transform.parent = LayersHolder.transform;
        Layer layer = layerObject.AddComponent<Layer>();
        layer.GenerateChunks(chunksInAxis, chunkPrefab);
        layers.Add(layer);
        return layer;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && showBoundGizmo)
        {
            foreach (Chunk chunk in _activeLayer.chunks)
            {
                if (!activeChunks.Contains(chunk))
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(chunk.center, Vector3.one * chunk.boundSize);
                }

            }
            foreach (Chunk chunk in activeChunks)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(chunk.center, Vector3.one * chunk.boundSize);
            }
        }
    }
}
