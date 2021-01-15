using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class JsonSerializer
{
    public static void Serialize(string path)
    {
        string tempName = path;
        int counter = 1;
        bool nameChanged = false;
        if (File.Exists(tempName + ".abs"))
        {
            while (File.Exists($"{tempName}{counter}.abs")) counter++;
            nameChanged = true;
            tempName = tempName + counter.ToString();
        }
        tempName += ".abs";

        StreamWriter sw = new StreamWriter(tempName);
        sw.Write(JsonUtility.ToJson(new JsonModel()));
        sw.Close();

        if (!nameChanged) UIController.Instance.ShowMessageBox("Model saved as " + Path.GetFileName(tempName));
        else UIController.Instance.ShowMessageBox($"File {Path.GetFileName(path)}.abs already existed.\nModel saved as {Path.GetFileName(tempName)}.");
    }
    public static void Deserialize(string path)
    {
        StreamReader sr = new StreamReader(path);
        JsonModel model = JsonUtility.FromJson<JsonModel>(sr.ReadToEnd());
        sr.Close();
        if (model.Layers == null || model.Layers.Length == 0) return;
        LayerManager.Instance.ClearLayers();
        for (int i = 0; i < model.Layers.Length; i++)
        {
            LayerManager.Instance.AddPreparedLayer(model.Layers[i]);
        }
        LayerManager.Instance.ActiveLayer=LayerManager.Instance.layers[LayerManager.Instance.layers.Count-1];
    }
}

[Serializable]
public class JsonModel
{
    public JsonLayer[] Layers;
    public JsonModel()
    {
        List<JsonLayer> list = new List<JsonLayer>();
        for (int i = 0; i < LayerManager.Instance.layers.Count; i++)
        {
            list.Add(new JsonLayer(LayerManager.Instance.layers[i]));
        }
        Layers = list.ToArray();
    }
}
[Serializable]
public class JsonLayer
{
    public JsonChunk[] Chunks;
    public string Name;
    public float Size;
    public int Resolution;
    public int ChunkResolution;
    public float Smoothness;
    public float Metallic;
    public int RenderType;
    public JsonQuaternion Rotation;
    public JsonVector3 Position;
    public JsonVector3 Scale;
    public JsonLayer(Layer l)
    {
        List<JsonChunk> list = new List<JsonChunk>();
        for (int i = 0; i < l.Resolution; i++)
        {
            for (int j = 0; j < l.Resolution; j++)
            {
                for (int k = 0; k < l.Resolution; k++)
                {
                    if (l.chunks[i, j, k].voxels.Initialized)
                    {
                        JsonChunk jsonChunk = new JsonChunk(l.chunks[i, j, k]);
                        if (jsonChunk.Values != null && jsonChunk.Values.Length != 0) list.Add(jsonChunk);
                    }
                }
            }
        }
        Chunks = list.ToArray();
        Name = l.name;
        Rotation = new JsonQuaternion(l.transform.rotation);
        Position = new JsonVector3(l.transform.position);
        Scale = new JsonVector3(l.transform.localScale);
        Size = l.Size;
        Smoothness = l.Smoothness;
        Metallic = l.Metallic;
        Resolution = l.Resolution;
        ChunkResolution = l.ChunkResolution;
        RenderType = (int)l.RenderType;
    }
}
[Serializable]
public class JsonChunk
{
    public JsonVoxel[] Values;
    public int x;
    public int y;
    public int z;
    public JsonChunk(Chunk c)
    {
        var vals = new float[c.resolution * c.resolution * c.resolution];
        var cols = new float[3 * c.resolution * c.resolution * c.resolution];

        c.voxels.VoxelBuffer.GetData(vals);
        c.voxels.ColorBuffer.GetData(cols);

        List<JsonVoxel> list = new List<JsonVoxel>();
        float maxValue = LayerManager.Instance.VoxelSpacing;
        for (int i = 0; i < vals.Length; i++)
        {
            if (vals[i] < maxValue) list.Add(new JsonVoxel(vals[i], i, cols[3 * i], cols[3 * i + 1], cols[3 * i + 2]));
        }
        vals = null;
        cols = null;
        Values = list.ToArray();
        x = c.coord.x;
        y = c.coord.y;
        z = c.coord.z;
    }
}
[Serializable]
public class JsonVector3
{
    public float x;
    public float y;
    public float z;
    public JsonVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
[Serializable]
public class JsonQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;
    public JsonQuaternion(Quaternion q)
    {
        x = q.x;
        y = q.y;
        z = q.z;
        w = q.w;
    }
    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}
[Serializable]
public class JsonVoxel
{
    public float v;
    public int k;
    public float c1;
    public float c2;
    public float c3;

    public JsonVoxel(float value, int index, float c1, float c2, float c3)
    {
        this.v = value;
        this.k = index;
        this.c1 = c1;
        this.c2 = c2;
        this.c3 = c3;
    }
}