using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    public Chunk[,,] chunks;

    public void GenerateChunks(int size, GameObject ChunkPrefab)
    {
        Chunk template = ChunkPrefab.GetComponent<Chunk>();
        chunks = new Chunk[size, size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    GameObject go = Instantiate(ChunkPrefab, transform);
                    go.name = $"Chunk ({x},{y},{z})";
                    Chunk chunk = go.GetComponent<Chunk>();
                    chunk.coord = new Vector3Int(x, y, z);
                    chunk.size = template.size;
                    BoxCollider col = go.GetComponent<BoxCollider>();
                    col.center = chunk.center;
                    col.size = Vector3.one * template.boundSize;
                    InitChunk(chunk);
                    chunks[x, y, z] = chunk;
                }
            }
        }
    }

    public void InitChunk(Chunk chunk)
    {
        chunk.voxelArray = new VoxelData[chunk.size * chunk.size * chunk.size];
        for (int x = 0; x < chunk.size; x++)
        {
            for (int y = 0; y < chunk.size; y++)
            {
                for (int z = 0; z < chunk.size; z++)
                {
                    int index = chunk.GetVoxelIndex(x, y, z);
                    float spacing = chunk.boundSize / (float)(chunk.size - 1);
                    Vector3 pos = chunk.center - Vector3.one * chunk.boundSize / 2f + new Vector3(x, y, z) * spacing;
                    chunk.voxelArray[index].position = pos;
                    chunk.voxelArray[index].value = 1f;
                }
            }
        }
    }
}
