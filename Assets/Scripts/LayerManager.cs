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
    [SerializeField]
    private float _size = 1;

    [SerializeField]
    private int _resolution = 3;

    [SerializeField]
    private int _chunkResolution = 32;

    public int Resolution => _resolution;
    public float Size => _size;

    public int ChunkResolution => _chunkResolution;

    public float Spacing => Size / Resolution;

    [Header("Gizmos")]
    [SerializeField]
    private bool _drawBoundingBox = false;
    [SerializeField]
    private bool _drawGrid = true;


    [SerializeField]
    private bool _drawOnlyActive = true;

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
        chunk.size = Spacing;
        chunk.resolution = ChunkResolution;
        _activeLayer = AddNewLayer();
    }

    Layer AddNewLayer()
    {
        GameObject layerObject = new GameObject($"Layer {layers.Count + 1}");
        layerObject.transform.parent = LayersHolder.transform;
        Layer layer = layerObject.AddComponent<Layer>();
        layer.Resolution = Resolution;
        layer.ChunkResolution = ChunkResolution;
        layer.Size = Size;
        layer.GenerateChunks(chunkPrefab);
        layers.Add(layer);
        return layer;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if(_drawGrid)
            {
                if(!_drawOnlyActive)
                {
                    foreach (Chunk chunk in _activeLayer.chunks)
                    {
                        if (!activeChunks.Contains(chunk))
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawWireCube(chunk.center, Vector3.one * chunk.size);
                        }

                    }
                }
                foreach (Chunk chunk in activeChunks)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(chunk.center, Vector3.one * chunk.size);
                }

            }
            if(_drawBoundingBox)
            {
                Gizmos.color = Color.black;
                //Gizmos.DrawWireCube(transform.position, Vector3.one * Size / 2f);
            }
        }
    }
}
