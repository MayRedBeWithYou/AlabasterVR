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

    public void GenerateChunks(GameObject ChunkPrefab)
    {
        chunks = new Chunk[Resolution, Resolution, Resolution];

        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                for (int z = 0; z < Resolution; z++)
                {
                    var foo = LayerManager.Instance.VoxelSpacing;
                    GameObject go = Instantiate(ChunkPrefab, transform);
                    go.transform.localPosition = new Vector3(x, y, z) * (foo*(ChunkResolution - 3));
                    //go.transform.localPosition = new Vector3(x * (Spacing - foo), y * (Spacing - foo), z * (Spacing - foo));
                    go.name = $"Chunk ({x},{y},{z})";
                    Chunk chunk = go.GetComponent<Chunk>();
                    chunk.coord = new Vector3Int(x, y, z);
                    chunk.resolution = ChunkResolution;
                    chunk.size = Spacing;
                    BoxCollider col = go.GetComponent<BoxCollider>();
                    col.center = Vector3.one * Spacing / 2f;
                    col.size = Vector3.one * Spacing;
                    chunk.Init();
                    chunks[x, y, z] = chunk;
                }
            }
        }
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
}
