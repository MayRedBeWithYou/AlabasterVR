// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="LayerManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum RenderType
{
    /// <summary>
    /// The flat
    /// </summary>
    Flat,
    /// <summary>
    /// The smooth
    /// </summary>
    Smooth
}

/// <summary>
/// Class LayerManager.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class LayerManager : MonoBehaviour
{
    private static LayerManager _instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static LayerManager Instance => _instance;

    /// <summary>
    /// The layers holder
    /// </summary>
    [Header("References")]

    public GameObject LayersHolder;

    /// <summary>
    /// The layer prefab
    /// </summary>
    public GameObject LayerPrefab;

    /// <summary>
    /// Gets the layers.
    /// </summary>
    /// <value>The layers.</value>
    public List<Layer> layers { get; private set; }

    /// <summary>
    /// The chunk prefab
    /// </summary>
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

    /// <summary>
    /// Gets the resolution.
    /// </summary>
    /// <value>The resolution.</value>
    public int Resolution => _resolution;
    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public float Size => _size;

    /// <summary>
    /// Gets the chunk resolution.
    /// </summary>
    /// <value>The chunk resolution.</value>
    public int ChunkResolution => _chunkResolution;
    /// <summary>
    /// Gets the size of the relative model.
    /// </summary>
    /// <value>The size of the relative model.</value>
    public float RelativeModelSize => _relativeModelSize;

    /// <summary>
    /// Gets the spacing.
    /// </summary>
    /// <value>The spacing.</value>
    public float Spacing => Size / Resolution;
    /// <summary>
    /// The voxel spacing
    /// </summary>
    public float VoxelSpacing;

    /// <summary>
    /// The metallic
    /// </summary>
    [Range(0f, 1f)]
    public float Metallic;

    /// <summary>
    /// The smoothness
    /// </summary>
    [Range(0f, 1f)]
    public float Smoothness;

    /// <summary>
    /// The render type
    /// </summary>
    public RenderType renderType;



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

    /// <summary>
    /// The active chunks
    /// </summary>
    public List<Chunk> activeChunks;

    /// <summary>
    /// Gets or sets the active layer.
    /// </summary>
    /// <value>The active layer.</value>
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

    /// <summary>
    /// Delegate LayerChange
    /// </summary>
    /// <param name="layer">The layer.</param>
    public delegate void LayerChange(Layer layer);

    /// <summary>
    /// Occurs when [active layer changed].
    /// </summary>
    public static event LayerChange ActiveLayerChanged;

    /// <summary>
    /// Occurs when [layer added].
    /// </summary>
    public static event LayerChange LayerAdded;

    /// <summary>
    /// Occurs when [layer removed].
    /// </summary>
    public static event LayerChange LayerRemoved;

    private int _layerCount = 0;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
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

    /// <summary>
    /// Adds the new layer.
    /// </summary>
    /// <returns>Layer.</returns>
    public Layer AddNewLayer()
    {
        GameObject layerObject = Instantiate(LayerPrefab, LayersHolder.transform);
        layerObject.name = $"Layer {++_layerCount}";
        layerObject.transform.position = LayersHolder.transform.position;
        Layer layer = layerObject.GetComponent<Layer>();
        layer.Resolution = Resolution;
        layer.ChunkResolution = ChunkResolution;
        layer.Size = Size;
        layer.Smoothness = Smoothness;
        layer.Metallic = Metallic;
        layer.RenderType = renderType;

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

    /// <summary>
    /// Adds the prepared layer.
    /// </summary>
    /// <param name="l">The l.</param>
    /// <returns>Layer.</returns>
    public Layer AddPreparedLayer(JsonLayer l)
    {
        _layerCount++;
        _size = l.Size;
        _chunkResolution = l.ChunkResolution;
        _resolution = l.Resolution;
        VoxelSpacing = Size / Resolution / (ChunkResolution - 1);
        GameObject layerObject = Instantiate(LayerPrefab, LayersHolder.transform);
        layerObject.name = l.Name;
        layerObject.transform.position = LayersHolder.transform.position;
        Layer layer = layerObject.GetComponent<Layer>();
        layer.Resolution = l.Resolution;
        layer.ChunkResolution = l.ChunkResolution;
        layer.Size = l.Size;
        layer.Smoothness = l.Smoothness;
        layer.Metallic = l.Metallic;
        layer.RenderType = (RenderType)l.RenderType;
        layer.SetPosition(l.Position.ToVector3());
        layer.SetRotation(l.Rotation.ToQuaternion());
        layer.SetScale(l.Scale.ToVector3());
        BoxCollider box = layerObject.GetComponent<BoxCollider>();
        box.size = Vector3.one * Size;
        box.center = Vector3.one * Size / 2;

        layer.GenerateChunks(chunkPrefab);
        for (int i = 0; i < l.Chunks.Length; i++)
        {
            float[] vals = Enumerable.Repeat(0.01209677f, layer.ChunkResolution * layer.ChunkResolution * layer.ChunkResolution).ToArray();
            float[] cols = Enumerable.Repeat(1.0f, layer.ChunkResolution * layer.ChunkResolution * layer.ChunkResolution * 3).ToArray();
            for (int j = 0; j < l.Chunks[i].Values.Length; j++)
            {
                JsonVoxel jsonVoxel = l.Chunks[i].Values[j];
                vals[jsonVoxel.k] = jsonVoxel.v;
                cols[3 * jsonVoxel.k] = jsonVoxel.c1;
                cols[3 * jsonVoxel.k + 1] = jsonVoxel.c2;
                cols[3 * jsonVoxel.k + 2] = jsonVoxel.c3;
            }
            Vector3Int ind=new Vector3Int(l.Chunks[i].x,l.Chunks[i].y,l.Chunks[i].z);
            layer.chunks[ind.x,ind.y,ind.z].voxels.InitializeFromArray(vals, cols);
            layer.chunks[ind.x,ind.y,ind.z].gpuMesh.UpdateVertexBuffer(layer.chunks[ind.x,ind.y,ind.z].voxels);
        }
        layers.Add(layer);
        LayerAdded?.Invoke(layer);
        Debug.Log($"Created layer: {layer.name}");

        if (ActiveLayer != null)
            ActiveLayer = layer;
        return layer;
    }

    /// <summary>
    /// Removes the layer.
    /// </summary>
    /// <param name="layer">The layer.</param>
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

    /// <summary>
    /// Resets the layers.
    /// </summary>
    public void ResetLayers()
    {
        ClearLayers();
        _activeLayer = AddNewLayer();
    }
    /// <summary>
    /// Clears the layers.
    /// </summary>
    public void ClearLayers()
    {
        foreach (Layer layer in layers)
        {
            Destroy(layer.gameObject);
        }
        layers.Clear();
        _layerCount = 0;
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

    /// <summary>
    /// Snaps to grid position.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <returns>Vector3Int.</returns>
    public Vector3Int SnapToGridPosition(Vector3 pos)
    {
        var layerPos = ActiveLayer.transform.position;
        //layerPos.x = layerPos.x % VoxelSpacing;
        //layerPos.y = layerPos.y % VoxelSpacing;
        //layerPos.z = layerPos.z % VoxelSpacing;

        return Vector3Int.RoundToInt((pos + layerPos) / VoxelSpacing);
    }

    /// <summary>
    /// Snaps to grid position real.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <returns>Vector3.</returns>
    public Vector3 SnapToGridPositionReal(Vector3 pos)
    {
        var layerPos = ActiveLayer.transform.position;
        layerPos.x = layerPos.x % VoxelSpacing;
        layerPos.y = layerPos.y % VoxelSpacing;
        layerPos.z = layerPos.z % VoxelSpacing;

        return ((Vector3)(Vector3Int.FloorToInt(pos / VoxelSpacing)) + Vector3.one * 0.5f) * VoxelSpacing + layerPos;
    }
}
