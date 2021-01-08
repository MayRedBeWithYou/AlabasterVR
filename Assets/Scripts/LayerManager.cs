using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    private static LayerManager _instance;

    public static LayerManager Instance => _instance;

    [Header("References")]

    public GameObject LayersHolder;

    public GameObject LayerPrefab;

    public List<Layer> layers { get; private set; }

    public GameObject chunkPrefab;

    [Header("Parameters")]
    [SerializeField]
    private float _size = 1;

    [SerializeField]
    private int _resolution = 3;

    [SerializeField]
    private int _chunkResolution = 32;
    [SerializeField]
    private float _relativeModelSize = 0.3f;

    public int Resolution => _resolution;
    public float Size => _size;

    public int ChunkResolution => _chunkResolution;
    public float RelativeModelSize => _relativeModelSize;

    public float Spacing => Size / Resolution;
    public float VoxelSpacing;

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

    public Layer ActiveLayer
    {
        get => _activeLayer;
        set
        {
            foreach (Chunk chunk in _activeLayer.chunks) chunk.ToggleColliders(false);
            activeChunks.Clear();
            _activeLayer = value;
            foreach (Chunk chunk in _activeLayer.chunks) chunk.ToggleColliders(true);
            Debug.Log($"Active layer: {value.name}");
            ActiveLayerChanged?.Invoke(value);
        }
    }

    public delegate void LayerChange(Layer layer);

    public static event LayerChange ActiveLayerChanged;

    public static event LayerChange LayerAdded;

    public static event LayerChange LayerRemoved;

    private int _layerCount = 0;

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
        VoxelSpacing = Size / Resolution / (ChunkResolution - 1);//Size / (Resolution * ChunkResolution);
        Chunk chunk = chunkPrefab.GetComponent<Chunk>();
        chunk.size = Spacing;
        chunk.resolution = ChunkResolution;
        _activeLayer = AddNewLayer();
    }

    public Layer AddNewLayer()
    {
        GameObject layerObject = Instantiate(LayerPrefab, LayersHolder.transform);
        layerObject.name = $"Layer {++_layerCount}";
        layerObject.transform.position = LayersHolder.transform.position;
        Layer layer = layerObject.GetComponent<Layer>();
        layer.Resolution = Resolution;
        layer.ChunkResolution = ChunkResolution;
        layer.Size = Size;

        BoxCollider box = layerObject.GetComponent<BoxCollider>();
        box.size = Vector3.one * Size;
        box.center = Vector3.one * Size / 2;

        layer.GenerateChunks(chunkPrefab);
        layers.Add(layer);
        LayerAdded?.Invoke(layer);
        Debug.Log($"Created layer: {layer.name}");

        if (ActiveLayer != null)
            ActiveLayer = layer;
        return layer;
    }

    public void RemoveLayer(Layer layer)
    {
        if (layers.Count == 1) return;

        int index = layers.IndexOf(layer);
        layers.RemoveAt(index);
        ActiveLayer = layers[Mathf.Clamp(index, 0, layers.Count - 1)];
        Debug.Log($"Removed layer: {layer.name}");
        Destroy(layer.gameObject);
        OperationManager.Instance.Clear();
        LayerRemoved?.Invoke(layer);
    }

    public void ResetLayers()
    {
        foreach (Layer layer in layers)
        {
            Destroy(layer.gameObject);
        }
        layers.Clear();
        _layerCount = 0;
        _activeLayer = AddNewLayer();
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (_drawGrid)
            {
                if (!_drawOnlyActive)
                {
                    foreach (Chunk chunk in _activeLayer.chunks)
                    {
                        if (!activeChunks.Contains(chunk))
                        {
                            Gizmos.color = Color.white;
                            Gizmos.matrix = chunk.transform.localToWorldMatrix;
                            Gizmos.DrawWireCube(Vector3.one * chunk.size / 2f, Vector3.one * chunk.size);
                        }

                    }
                }
                foreach (Chunk chunk in activeChunks)
                {
                    Gizmos.color = Color.green;
                    Gizmos.matrix = chunk.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(Vector3.one * chunk.size / 2f, Vector3.one * chunk.size);
                }

            }
            if (_drawBoundingBox)
            {
                Gizmos.color = Color.black;
                //Gizmos.DrawWireCube(transform.position, Vector3.one * Size / 2f);
            }
        }
    }

    public Vector3Int SnapToGridPosition(Vector3 pos)
    {
        var layerPos = ActiveLayer.transform.position;
        //layerPos.x = layerPos.x % VoxelSpacing;
        //layerPos.y = layerPos.y % VoxelSpacing;
        //layerPos.z = layerPos.z % VoxelSpacing;

        return Vector3Int.RoundToInt((pos + layerPos) / VoxelSpacing);
    }

    public Vector3 SnapToGridPositionReal(Vector3 pos)
    {
        var layerPos = ActiveLayer.transform.position;
        layerPos.x = layerPos.x % VoxelSpacing;
        layerPos.y = layerPos.y % VoxelSpacing;
        layerPos.z = layerPos.z % VoxelSpacing;

        return ((Vector3)(Vector3Int.FloorToInt(pos / VoxelSpacing)) +Vector3.one * 0.5f) * VoxelSpacing + layerPos ;
    }
}
