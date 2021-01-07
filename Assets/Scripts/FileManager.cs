using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Linq;
public static class FileManager
{
    public static FileExplorer SaveModel(FileExplorer script)
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

    public static FileExplorer LoadModel(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".obj" });
        script.UpdateDirectory();
        script.OnAccepted += (text) =>
          {
              if (System.String.IsNullOrWhiteSpace(text)) return;
              script.Close();
          };
        //todo:meshtosdf

        return script;
    }

    public static FileExplorer LoadImageReference(FileExplorer script)
    {
        script.mode = FileExplorerMode.Open;
        script.SetExtensionsArray(new string[] { ".jpg", ".png" });
        script.UpdateDirectory();
        return script;
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

        foreach (var l in LayerManager.Instance.layers)
        {
            foreach (var chunk in l.chunks)
            {
                if (chunk.voxels.Initialized)
                {
                    var tris = chunk.gpuMesh.GetTriangles();
                    for (int i = 0; i < tris.Length; i++)
                    {
                        var norm = chunk.ModelMatrix.MultiplyVector(tris[i].normA);

                        ObjVertex[] tmp = new ObjVertex[3];
                        tmp[0].coordinates = chunk.ModelMatrix.MultiplyPoint(tris[i].vertexA);
                        tmp[1].coordinates = chunk.ModelMatrix.MultiplyPoint(tris[i].vertexB);
                        tmp[2].coordinates = chunk.ModelMatrix.MultiplyPoint(tris[i].vertexC);
                        ObjTriangle tri;
                        tri.verts = new int[3];
                        for (int j = 0; j < tmp.Length; j++)
                        {
                            if (vertices.ContainsKey(tmp[j].coordinates))
                            {
                                var v = vertices[tmp[j].coordinates];
                                v.normal += tmp[j].normal;
                                v.faces++;
                                tri.verts[j] = v.index;
                                vertices[tmp[j].coordinates] = v;
                            }
                            else
                            {
                                tmp[j].normal = norm;
                                tmp[j].index = vertexIndex;
                                tmp[j].faces = 1;
                                vertices.Add(tmp[j].coordinates, tmp[j]);
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

        foreach (var item in vertices)
        {
            float3 n;
            n.coord = item.Value.normal;
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
        foreach (var item in vertices) stringBuilder.AppendLine(item.Value.ToStringVertex());
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
        sw.WriteLine("#faces count: " + vertices.Count + Environment.NewLine);
        sw.Close();

        if (!nameChanged) UIController.Instance.ShowMessageBox("Model zapisano jako " + Path.GetFileName(tempName));
        else UIController.Instance.ShowMessageBox($"Model o nazwie {Path.GetFileName(path)}.obj już istniał.\nModel zapisano jako {Path.GetFileName(tempName)}.");
    }

    struct ObjVertex
    {
        public int index;
        public int faces;
        public Vector3 coordinates;
        public Vector3 normal;

        public override bool Equals(object obj)
        {
            ObjVertex vertex = (ObjVertex)obj;
            return coordinates.Equals(vertex.coordinates);
        }
        public override int GetHashCode()
        {
            return coordinates.GetHashCode();
        }
        public string ToStringVertex()
        {
            System.Globalization.CultureInfo cInfo = System.Globalization.CultureInfo.InvariantCulture;
            return "v " + coordinates.x.ToString(cInfo) + " " + coordinates.y.ToString(cInfo) + " " + coordinates.z.ToString(cInfo);
            //return String.Format("v {0} {1} {2}", coordinates.x.ToString(System.Globalization.CultureInfo.InvariantCulture), coordinates.y.ToString(System.Globalization.CultureInfo.InvariantCulture), coordinates.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        public string ToStringNormals()
        {
            System.Globalization.CultureInfo cInfo = System.Globalization.CultureInfo.InvariantCulture;
            return String.Format("vn {0} {1} {2}", normal.x.ToString(System.Globalization.CultureInfo.InvariantCulture), normal.y.ToString(System.Globalization.CultureInfo.InvariantCulture), normal.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
    struct ObjTriangle
    {
        public int[] verts;
        public override bool Equals(object obj)
        {
            ObjTriangle tmp = (ObjTriangle)obj;
            return tmp.verts[0] == verts[0] && tmp.verts[1] == verts[1] && tmp.verts[2] == verts[2];
        }
        public override int GetHashCode()
        {
            return verts[0].GetHashCode() ^ verts[1].GetHashCode() ^ verts[2].GetHashCode();
        }

        public string ToStringCustom(int n1, int n2, int n3)
        {
            System.Globalization.CultureInfo cInfo = System.Globalization.CultureInfo.InvariantCulture;
            string result = "f " + verts[0].ToString(cInfo) + "//" + n1.ToString(cInfo) +
            " " + verts[1].ToString(cInfo) + "//" + n2.ToString(cInfo) + " " + verts[2].ToString(cInfo) + "//" + n3.ToString(cInfo);

            return result;
            //return String.Format("f {0}//{0} {1}//{1} {2}//{2}", verts[0].ToString(System.Globalization.CultureInfo.InvariantCulture), verts[1].ToString(System.Globalization.CultureInfo.InvariantCulture), verts[2].ToString(System.Globalization.CultureInfo.InvariantCulture));
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

