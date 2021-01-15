// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="JsonSerializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Class JsonSerializer.
/// </summary>
public static class JsonSerializer
{
    /// <summary>
    /// Serializes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
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
    /// <summary>
    /// Deserializes the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
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

/// <summary>
/// Class JsonModel.
/// </summary>
[Serializable]
public class JsonModel
{
    /// <summary>
    /// The layers
    /// </summary>
    public JsonLayer[] Layers;
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonModel" /> class.
    /// </summary>
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
/// <summary>
/// Class JsonLayer.
/// </summary>
[Serializable]
public class JsonLayer
{
    /// <summary>
    /// The chunks
    /// </summary>
    public JsonChunk[] Chunks;
    /// <summary>
    /// The name
    /// </summary>
    public string Name;
    /// <summary>
    /// The size
    /// </summary>
    public float Size;
    /// <summary>
    /// The resolution
    /// </summary>
    public int Resolution;
    /// <summary>
    /// The chunk resolution
    /// </summary>
    public int ChunkResolution;
    /// <summary>
    /// The smoothness
    /// </summary>
    public float Smoothness;
    /// <summary>
    /// The metallic
    /// </summary>
    public float Metallic;
    /// <summary>
    /// The render type
    /// </summary>
    public int RenderType;
    /// <summary>
    /// The rotation
    /// </summary>
    public JsonQuaternion Rotation;
    /// <summary>
    /// The position
    /// </summary>
    public JsonVector3 Position;
    /// <summary>
    /// The scale
    /// </summary>
    public JsonVector3 Scale;
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLayer" /> class.
    /// </summary>
    /// <param name="l">The l.</param>
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
/// <summary>
/// Class JsonChunk.
/// </summary>
[Serializable]
public class JsonChunk
{
    /// <summary>
    /// The values
    /// </summary>
    public JsonVoxel[] Values;
    /// <summary>
    /// The x
    /// </summary>
    public int x;
    /// <summary>
    /// The y
    /// </summary>
    public int y;
    /// <summary>
    /// The z
    /// </summary>
    public int z;
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonChunk" /> class.
    /// </summary>
    /// <param name="c">The c.</param>
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
/// <summary>
/// Class JsonVector3.
/// </summary>
[Serializable]
public class JsonVector3
{
    /// <summary>
    /// The x
    /// </summary>
    public float x;
    /// <summary>
    /// The y
    /// </summary>
    public float y;
    /// <summary>
    /// The z
    /// </summary>
    public float z;
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonVector3" /> class.
    /// </summary>
    /// <param name="v">The v.</param>
    public JsonVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    /// <summary>
    /// Converts to vector3.
    /// </summary>
    /// <returns>Vector3.</returns>
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
/// <summary>
/// Class JsonQuaternion.
/// </summary>
[Serializable]
public class JsonQuaternion
{
    /// <summary>
    /// The x
    /// </summary>
    public float x;
    /// <summary>
    /// The y
    /// </summary>
    public float y;
    /// <summary>
    /// The z
    /// </summary>
    public float z;
    /// <summary>
    /// The w
    /// </summary>
    public float w;
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonQuaternion" /> class.
    /// </summary>
    /// <param name="q">The q.</param>
    public JsonQuaternion(Quaternion q)
    {
        x = q.x;
        y = q.y;
        z = q.z;
        w = q.w;
    }
    /// <summary>
    /// Converts to quaternion.
    /// </summary>
    /// <returns>Quaternion.</returns>
    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}
/// <summary>
/// Class JsonVoxel.
/// </summary>
[Serializable]
public class JsonVoxel
{
    /// <summary>
    /// The v
    /// </summary>
    public float v;
    /// <summary>
    /// The k
    /// </summary>
    public int k;
    /// <summary>
    /// The c1
    /// </summary>
    public float c1;
    /// <summary>
    /// The c2
    /// </summary>
    public float c2;
    /// <summary>
    /// The c3
    /// </summary>
    public float c3;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonVoxel" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The index.</param>
    /// <param name="c1">The c1.</param>
    /// <param name="c2">The c2.</param>
    /// <param name="c3">The c3.</param>
    public JsonVoxel(float value, int index, float c1, float c2, float c3)
    {
        this.v = value;
        this.k = index;
        this.c1 = c1;
        this.c2 = c2;
        this.c3 = c3;
    }
}