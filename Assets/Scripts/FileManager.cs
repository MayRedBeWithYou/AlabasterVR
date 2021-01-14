using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Globalization;
public static class FileManager
{
    public static int precisionMultiplier = 100;
    public static bool noVertexNormals;

    public static FileExplorer SaveModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Save;
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              JsonSerializer.Serialize(text);
              script.Close();
          };
        return script;
    }
    public static FileExplorer LoadModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".abs" });
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              JsonSerializer.Deserialize(text);
              script.Close();
          };
        return script;
    }
    public static FileExplorer ExportModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Save;
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              TranslateModelToObj(text);
              script.Close();
          };
        return script;
    }
    public static FileExplorer ImportModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".obj" });
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              try
              {
                  MeshToSdfGpu.TranslateTrianglesToSdf(TranslateObjToModel(text), true);
              }
              catch
              {
                  UIController.Instance.ShowMessageBox("Model couldn't be loaded - incorrect file format");
              }
              script.Close();
          };
        return script;
    }

    public static FileExplorer LoadImageReference(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".jpg", ".png" });
        script.UpdateDirectory();
        return script;
    }
    private static MeshToSdfGpu.GPUTriangle[] TranslateObjToModel(string path)
    {
        StreamReader streamReader = new StreamReader(path);
        List<int[]> trianglesIndices = new List<int[]>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        noVertexNormals = false;
        while (!streamReader.EndOfStream)
        {
            string current = streamReader.ReadLine();
            if (current.Length > 2)
            {
                if (current[0] == 'f')
                {
                    var temp = ParseToFace(current);
                    for (int i = 0; i < temp.Count / 3; i++)
                    {
                        int[] triIndices = new int[6];
                        for (int j = 0; j < 3; j++)
                        {
                            if (temp[3 * i + j].x > 0) triIndices[j] = temp[3 * i + j].x - 1;
                            else triIndices[j] = vertices.Count + temp[3 * i + j].x;

                            if (temp[3 * i + j].y > 0) triIndices[3 + j] = temp[3 * i + j].y - 1;
                            else if (temp[3 * i + j].y < 0) triIndices[3 + j] = normals.Count + temp[3 * i + j].y;
                            else noVertexNormals = true;
                        }
                        trianglesIndices.Add(triIndices);
                    }
                }
                else if (current[0] == 'v')
                {
                    if (current[1] == 'n') normals.Add(ParseToVector3(current));
                    else if (current[1] == ' ') vertices.Add(ParseToVector3(current));
                }
            }
        }
        streamReader.Close();

        Vector3 min = Vector3.positiveInfinity, max = Vector3.negativeInfinity;
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                min = Vector3.Min(min, vertices[i]);
                max = Vector3.Max(max, vertices[i]);
            }
        }
        Vector3 diff = max - min;
        float meshDiameter = Mathf.Max(diff.x, diff.y, diff.z);
        float scale = (LayerManager.Instance.RelativeModelSize * LayerManager.Instance.Size) / meshDiameter;
        Vector3 sceneCenter = Vector3.one * (LayerManager.Instance.Size * 0.5f);
        Vector3 centeringVector = sceneCenter - diff * scale * 0.5f;
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] -= min;
            vertices[i] *= scale;
            vertices[i] += centeringVector;
        }
        min = centeringVector;
        max = diff * scale + centeringVector;
        Bounds boundsMine = new Bounds((min + max) * 0.5f, max - min);

        MeshToSdfGpu.bounds = new Bounds((min + max) * 0.5f, (max - min) + Vector3.one * LayerManager.Instance.Spacing / (LayerManager.Instance.ChunkResolution - 1) * 2);
        //Mesh mesh = new Mesh();
        //mesh.vertices = vertices.ToArray();

        Vector3[] normalsArray;
        if (noVertexNormals)
        {
            normalsArray = new Vector3[vertices.Count];
            for (int i = 0; i < trianglesIndices.Count; i++)
            {
                var tab = trianglesIndices[i];
                Vector3 faceNormal = Vector3.Cross(vertices[tab[1]] - vertices[tab[0]], vertices[tab[2]] - vertices[tab[0]]).normalized;
                normalsArray[tab[3]] += faceNormal;
                normalsArray[tab[4]] += faceNormal;
                normalsArray[tab[5]] += faceNormal;
            }
        }
        else normalsArray = normals.ToArray();
        normals = null;
        for (int i = 0; i < normalsArray.Length; i++) normalsArray[i] = normalsArray[i].normalized;

        MeshToSdfGpu.GPUTriangle[] result = new MeshToSdfGpu.GPUTriangle[trianglesIndices.Count];
        for (int i = 0; i < result.Length; i++)
        {
            var tab = trianglesIndices[i];
            result[i] = new MeshToSdfGpu.GPUTriangle()
            {
                vertexA = vertices[tab[0]],
                vertexB = vertices[tab[1]],
                vertexC = vertices[tab[2]],
                normA = normalsArray[tab[3]],
                normB = normalsArray[tab[4]],
                normC = normalsArray[tab[5]]
            };
        }
        return result;
    }
    private static void TranslateModelToObj(string path)
    {
        string tempName = path;
        int counter = 1;
        bool nameChanged = false;
        if (File.Exists(tempName + ".obj"))
        {
            while (File.Exists($"{tempName}{counter}.obj")) counter++;
            nameChanged = true;
            tempName = tempName + counter.ToString();
        }
        tempName += ".obj";
        int vertexIndex = 1;
        Dictionary<Vector3, ObjVertex> vertices = new Dictionary<Vector3, ObjVertex>();
        HashSet<ObjTriangle> triangles = new HashSet<ObjTriangle>();

        //get triangles from GPUMesh
        foreach (var l in LayerManager.Instance.layers)
        {
            foreach (var chunk in l.chunks)
            {
                if (chunk.voxels.Initialized)
                {
                    var tris = chunk.gpuMesh.GetTriangles();
                    for (int i = 0; i < tris.Length; i++)
                    {
                        var currentNorm = chunk.ModelMatrix.MultiplyVector(tris[i].normA).normalized;

                        ObjVertex[] tmp = new ObjVertex[3];
                        tmp[0].coord = chunk.ModelMatrix.MultiplyPoint(tris[i].vertexA);
                        tmp[1].coord = chunk.ModelMatrix.MultiplyPoint(tris[i].vertexB);
                        tmp[2].coord = chunk.ModelMatrix.MultiplyPoint(tris[i].vertexC);
                        ObjTriangle tri;
                        tri.verts = new int[3];
                        for (int j = 0; j < tmp.Length; j++)
                        {
                            if (vertices.ContainsKey(tmp[j].coord))
                            {
                                var v = vertices[tmp[j].coord];
                                v.normal += currentNorm;
                                tri.verts[j] = v.index;
                                vertices[tmp[j].coord] = v;
                            }
                            else
                            {
                                tmp[j].normal = currentNorm;
                                tmp[j].index = vertexIndex;
                                vertices.Add(tmp[j].coord, tmp[j]);
                                tri.verts[j] = vertexIndex;
                                vertexIndex++;
                            }
                        }
                        triangles.Add(tri);
                    }
                }
            }
        }
        int normInd = 1;
        int[] normalsArray = new int[vertices.Count];
        Dictionary<float3, int> normalsDict = new Dictionary<float3, int>();

        //reduce unnecessary normal vectors
        foreach (var item in vertices)
        {
            float3 n = new float3() { coord = item.Value.normal };
            int realIndex;
            if (normalsDict.TryGetValue(n, out realIndex))
            {
                normalsArray[item.Value.index - 1] = realIndex;
            }
            else
            {
                normalsDict.Add(n, normInd);
                normalsArray[item.Value.index - 1] = normInd;
                normInd++;
            }
        }
        StreamWriter sw = new StreamWriter(tempName);
        sw.WriteLine("#AlabasterVR" + Environment.NewLine);

        sw.WriteLine("#vertices");
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var item in vertices) stringBuilder.AppendLine(item.Value.ToString());
        sw.Write(stringBuilder.ToString());
        sw.WriteLine("#vertices count: " + vertices.Count + Environment.NewLine);

        sw.WriteLine("#vertex normals");
        stringBuilder = new StringBuilder();
        foreach (var item in normalsDict) stringBuilder.AppendLine(item.Key.ToString());
        sw.Write(stringBuilder.ToString());
        sw.WriteLine("#vertex normals count: " + normalsDict.Count + Environment.NewLine);

        sw.WriteLine("#faces");
        stringBuilder = new StringBuilder();
        foreach (var item in triangles) stringBuilder.AppendLine(item.ToStringCustom(normalsArray[item.verts[0] - 1], normalsArray[item.verts[1] - 1], normalsArray[item.verts[2] - 1]));
        sw.Write(stringBuilder.ToString());
        sw.WriteLine("#faces count: " + triangles.Count + Environment.NewLine);
        sw.Close();

        if (!nameChanged) UIController.Instance.ShowMessageBox("Model exported to " + Path.GetFileName(tempName));
        else UIController.Instance.ShowMessageBox($"File {Path.GetFileName(path)}.obj already existed.\nModel exported to {Path.GetFileName(tempName)}.");
    }

    private static Vector3 ParseToVector3(string text)
    {
        float f1, f2, f3;
        string[] tmp;
        char[] sep = new char[] { ' ' };
        tmp = text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        float.TryParse(tmp[1], NumberStyles.Float, CultureInfo.InvariantCulture, out f1);
        float.TryParse(tmp[2], NumberStyles.Float, CultureInfo.InvariantCulture, out f2);
        float.TryParse(tmp[3], NumberStyles.Float, CultureInfo.InvariantCulture, out f3);

        return new Vector3(f1, f2, f3);
    }

    private static List<Vector2Int> ParseToFace(string text)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        string[] tmp;
        char[] sep = new char[] { ' ' };
        tmp = text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < tmp.Length; i++)
        {
            int v, vn;
            int firstInd = tmp[i].IndexOf('/');
            int lastInd = tmp[i].LastIndexOf('/');

            string s1 = tmp[i].Substring(0, firstInd);
            int.TryParse(s1, out v);

            if (firstInd == lastInd)
            {
                noVertexNormals = true;
                vn = 0;
            }
            else
            {
                string s2 = tmp[i].Substring(lastInd + 1);
                int.TryParse(s2, out vn);
            }

            if (result.Count < 3) result.Add(new Vector2Int(v, vn));
            else
            {
                result.Add(result[0]);
                result.Add(result[result.Count - 2]);
                result.Add(new Vector2Int(v, vn));
            }
        }
        return result;
    }

    struct ObjVertex
    {
        public int index;
        public Vector3 coord;
        public Vector3 normal;

        public override bool Equals(object obj)
        {
            ObjVertex tmp = (ObjVertex)obj;
            return coord.x.Equals(tmp.coord.x) && coord.y.Equals(tmp.coord.y) && coord.z.Equals(tmp.coord.z);
        }
        public override int GetHashCode()
        {
            return coord.x.GetHashCode() ^ coord.y.GetHashCode() ^ coord.z.GetHashCode();
        }
        public override string ToString()
        {
            System.Globalization.CultureInfo cInfo = System.Globalization.CultureInfo.InvariantCulture;
            return "v " + coord.x.ToString(cInfo) + " " + coord.y.ToString(cInfo) + " " + coord.z.ToString(cInfo);
        }
    }
    struct ObjTriangle
    {
        public int[] verts;
        public override bool Equals(object obj)
        {
            ObjTriangle tmp = (ObjTriangle)obj;
            return tmp.verts[0].Equals(verts[0]) && tmp.verts[1].Equals(verts[1]) && tmp.verts[2].Equals(verts[2]);
        }
        public override int GetHashCode()
        {
            return verts[0].GetHashCode() ^ verts[1].GetHashCode() ^ verts[2].GetHashCode();
        }
        public string ToStringCustom(int n1, int n2, int n3)
        {
            System.Globalization.CultureInfo cInfo = System.Globalization.CultureInfo.InvariantCulture;
            return "f " + verts[0].ToString(cInfo) + "//" + n1.ToString(cInfo) +
            " " + verts[1].ToString(cInfo) + "//" + n2.ToString(cInfo) + " " + verts[2].ToString(cInfo) + "//" + n3.ToString(cInfo);
        }
    }
    struct float3
    {
        public Vector3 coord;

        public override bool Equals(object obj)
        {
            float3 tmp = (float3)obj;
            return coord.x.Equals(tmp.coord.x) && coord.y.Equals(tmp.coord.y) && coord.z.Equals(tmp.coord.z);
        }
        public override int GetHashCode()
        {
            return coord.x.GetHashCode() ^ coord.y.GetHashCode() ^ coord.z.GetHashCode();
        }
        public override string ToString()
        {
            System.Globalization.CultureInfo cInfo = System.Globalization.CultureInfo.InvariantCulture;
            return "vn " + coord.x.ToString(cInfo) + " " + coord.y.ToString(cInfo) + " " + coord.z.ToString(cInfo);
        }
    }
}

