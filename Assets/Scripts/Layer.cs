using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
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
                    GameObject go = Instantiate(ChunkPrefab, transform);
                    go.transform.localPosition = new Vector3(x * Spacing, y * Spacing, z * Spacing);
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


    ////////////////////
    public void GenerateChunksFromMesh(GameObject ChunkPrefab, float[] values)
    {
        chunks = new Chunk[Resolution, Resolution, Resolution];

        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                for (int z = 0; z < Resolution; z++)
                {
                    GameObject go = Instantiate(ChunkPrefab, transform);
                    go.transform.localPosition = new Vector3(x * Spacing, y * Spacing, z * Spacing);
                    go.name = $"Chunk ({x},{y},{z})";
                    Chunk chunk = go.GetComponent<Chunk>();
                    chunk.coord = new Vector3Int(x, y, z);
                    chunk.resolution = ChunkResolution;
                    chunk.size = Spacing;
                    BoxCollider col = go.GetComponent<BoxCollider>();
                    col.center = Vector3.one * Spacing / 2f;
                    col.size = Vector3.one * Spacing;
                    float[]tmp=new float[ChunkResolution*ChunkResolution*ChunkResolution];
                    System.Buffer.BlockCopy(values,(z+y*Resolution+x*Resolution*Resolution)*ChunkResolution,tmp,0,sizeof(float)*tmp.Length);
                    chunk.InitFromMesh(tmp);
                    chunks[x, y, z] = chunk;
                }
            }
        }
    }

   
}
