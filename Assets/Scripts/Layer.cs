using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour, IMovable, IResizable
{
    [Header("Parameters")]

    public int Resolution;

    public float Size;

    public int ChunkResolution;
    public float Spacing => Size / Resolution;

    public Chunk[,,] chunks;
    private float metallic;
    private float smoothness;
    private RenderType renderType;

    private MeshFilter meshFilter;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public RenderType RenderType
    {
        get => renderType;
        set
        {
            renderType = value;
            if (chunks != null)
                foreach (Chunk c in chunks)
                {
                    c.gpuMesh.renderType = value;
                    if (c.voxels.Initialized) c.gpuMesh.UpdateVertexBuffer(c.voxels);
                }
        }
    }

    public float Metallic
    {
        get => metallic;
        set
        {
            metallic = value;
            if (chunks != null)
                foreach (Chunk c in chunks)
                {
                    c.gpuMesh.metallic = value;
                }
        }
    }

    public float Smoothness
    {
        get => smoothness;
        set
        {
            smoothness = value;
            if (chunks != null)
                foreach (Chunk c in chunks)
                {
                    c.gpuMesh.smoothness = value;
                }
        }
    }

    public void GenerateChunks(GameObject ChunkPrefab)
    {
        chunks = new Chunk[Resolution, Resolution, Resolution];

        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                for (int z = 0; z < Resolution; z++)
                {
                    GameObject go = Instantiate(ChunkPrefab, transform);
                    go.transform.localPosition = new Vector3(x, y, z) * (LayerManager.Instance.VoxelSpacing * (ChunkResolution - 3)); //chunkRes - 3, because chunks have to overlap each other if we want to compute gradient
                    go.name = $"Chunk ({x},{y},{z})";
                    Chunk chunk = go.GetComponent<Chunk>();
                    chunk.coord = new Vector3Int(x, y, z);
                    chunk.resolution = ChunkResolution;
                    chunk.size = Spacing;
                    BoxCollider col = go.GetComponent<BoxCollider>();
                    col.center = Vector3.one * Spacing / 2f;
                    col.size = Vector3.one * Spacing;
                    chunk.Init();
                    chunk.gpuMesh.renderType = renderType;
                    chunks[x, y, z] = chunk;
                }
            }
        }

        var mesh = new Mesh();

        Vector3[] indices = new Vector3[8]{new Vector3(0,0,0), new Vector3(1,0,0),new Vector3(1,0,1),new Vector3(0,0,1),
            new Vector3(0,1,0), new Vector3(1,1,0),new Vector3(1,1,1),new Vector3(0,1,1)};
        Vector3[] borderPoints = new Vector3[8];

        float fix = Size - (Resolution - 1) / (2f * ChunkResolution);

        for (int i = 0; i < indices.Length; i++)
        {
            borderPoints[i] = new Vector3(indices[i].x * fix, indices[i].y * fix, indices[i].z * fix);
        }
        mesh.vertices = borderPoints;
        mesh.SetIndices(new int[] { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 }, MeshTopology.Lines, 0, false);

        meshFilter.sharedMesh = mesh;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void ToggleRenderer(bool value)
    {
        meshRenderer.enabled = value;
    }
}
