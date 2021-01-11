using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public static class MeshToSdfGpu
{
    public struct GPUTriangle
    {
        public Vector3 vertexC;
        public Vector3 vertexA;
        public Vector3 vertexB;
        public Vector3 normC;
        public Vector3 normA;
        public Vector3 normB;
    };

    private static ComputeShader meshToSdfShader;
    private static ComputeBuffer trianglesBuffer;
    public static Bounds bounds;

    private static void Initialize(GPUTriangle[] triangles)
    {
        trianglesBuffer = new ComputeBuffer(triangles.Length * 18, sizeof(float));
        float[] arr=new float[18*triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            arr[18*i]=triangles[i].vertexC.x;
            arr[18*i+1]=triangles[i].vertexC.y;
            arr[18*i+2]=triangles[i].vertexC.z;
            arr[18*i+3]=triangles[i].vertexA.x;
            arr[18*i+4]=triangles[i].vertexA.y;
            arr[18*i+5]=triangles[i].vertexA.z;
            arr[18*i+6]=triangles[i].vertexB.x;
            arr[18*i+7]=triangles[i].vertexB.y;
            arr[18*i+8]=triangles[i].vertexB.z;
            arr[18*i+9]=triangles[i].normC.x;
            arr[18*i+10]=triangles[i].normC.y;
            arr[18*i+11]=triangles[i].normC.z;
            arr[18*i+12]=triangles[i].normA.x;
            arr[18*i+13]=triangles[i].normA.y;
            arr[18*i+14]=triangles[i].normA.z;
            arr[18*i+15]=triangles[i].normB.x;
            arr[18*i+16]=triangles[i].normB.y;
            arr[18*i+17]=triangles[i].normB.z;
        }
        trianglesBuffer.SetData(arr);

        //trianglesBuffer.GetData(arr);
    }
    private static void WriteData(float[] arr, string name)
    {
        StreamWriter sw = new StreamWriter(name + ".txt");
        for (int i = 0; i < arr.Length; i++)
        {
            sw.Write(arr[i].ToString()+' ');
        }
        sw.Close();
    }
    public static void TranslateTrianglesToSdf(GPUTriangle[] triangles, bool calculateOnGpu = true)
    {
        meshToSdfShader = Resources.Load<ComputeShader>("MeshToSdf/MeshToSdfVertexNormalsShader");
        Initialize(triangles);
        float[] arr = new float[LayerManager.Instance.ChunkResolution * LayerManager.Instance.ChunkResolution * LayerManager.Instance.ChunkResolution];
        LayerManager.Instance.AddNewLayer();
        int counter = 0;
        foreach (var chunk in LayerManager.Instance.ActiveLayer.chunks)
        {
            if (bounds.Intersects(chunk.ColliderBounds))
            {
                Debug.Log($"{counter}/ current: {LayerManager.Instance.Resolution * LayerManager.Instance.Resolution * LayerManager.Instance.Resolution}");
                counter++;
                Vector3 chunkOffset = new Vector3(chunk.coord.x * LayerManager.Instance.Spacing, chunk.coord.y * LayerManager.Instance.Spacing, chunk.coord.z * LayerManager.Instance.Spacing);
                chunk.voxels.Initialized = true;
                LayerManager.Instance.activeChunks.Add(chunk);
                if (calculateOnGpu)
                {
                    int kernel = meshToSdfShader.FindKernel("CalculateDistance");
                    meshToSdfShader.SetBuffer(kernel, "inputTriangles", trianglesBuffer);
                    meshToSdfShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
                    meshToSdfShader.SetMatrix("modelMatrix", chunk.ModelMatrix);
                    meshToSdfShader.SetInt("numPointsPerAxis", LayerManager.Instance.ChunkResolution);
                    meshToSdfShader.SetInt("trianglesCount", triangles.Length);
                    meshToSdfShader.SetFloat("chunkSize", LayerManager.Instance.Spacing);
                    meshToSdfShader.SetFloat("sizeSquared", LayerManager.Instance.Size * LayerManager.Instance.Size);
                    meshToSdfShader.SetFloat("maxValue", LayerManager.Instance.Spacing / (LayerManager.Instance.ChunkResolution - 1));


                    meshToSdfShader.Dispatch(kernel, chunk.voxels.Resolution / 8, chunk.voxels.Resolution / 8, chunk.voxels.Resolution / 8);
                    //chunk.voxels.VoxelBuffer.GetData(arr);
                    //WriteData(arr,chunk.name);
                }
                else
                {
                    M2SdfGpuCpu translator = new M2SdfGpuCpu(ref triangles, chunkOffset, LayerManager.Instance.ChunkResolution, triangles.Length, LayerManager.Instance.Spacing, LayerManager.Instance.Size, LayerManager.Instance.Spacing / (LayerManager.Instance.ChunkResolution - 1), chunk.ModelMatrix);
                    translator.Compute();
                    chunk.voxels.InitializeFromArray(translator.sdf);
                }
            }
        }
        Debug.Log(counter);
        foreach (var chunk in LayerManager.Instance.activeChunks) chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        trianglesBuffer.Release();
    }
}

class M2SdfGpuCpu
{
    MeshToSdfGpu.GPUTriangle[] inputTriangles;
    Vector3 chunkOffset;
    int numPointsPerAxis;
    int trianglesCount;
    float chunkSize;
    float size;
    float maxValue;
    Matrix4x4 modelMatrix;
    public float[] sdf;

    public M2SdfGpuCpu(ref MeshToSdfGpu.GPUTriangle[] triangles, Vector3 chunkOffset, int numPointsPerAxis, int trianglesCount, float chunkSize, float size, float maxValue, Matrix4x4 modelM)
    {
        this.inputTriangles = triangles;
        this.chunkOffset = chunkOffset;
        this.numPointsPerAxis = numPointsPerAxis;
        this.trianglesCount = trianglesCount;
        this.chunkSize = chunkSize;
        this.size = size;
        this.maxValue = maxValue;
        this.sdf = new float[numPointsPerAxis * numPointsPerAxis * numPointsPerAxis];
        modelMatrix = modelM;
    }

    int coords(Vector3Int id)
    {
        return id.z * numPointsPerAxis * numPointsPerAxis + id.y * numPointsPerAxis + id.x;
    }

    Vector3 realPosExternal(Vector3Int id)
    {
        float res = numPointsPerAxis - 1;
        Vector3 ids = (Vector3)id / res;
        return modelMatrix.MultiplyPoint(ids * chunkSize);
    }
    Vector3 Barycentric(Vector3 P, int index)
    {
        //https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates

        Vector3 result = new Vector3(P.x, P.y, P.z);
        Vector3 v0 = inputTriangles[index].vertexB - inputTriangles[index].vertexA;
        Vector3 v1 = inputTriangles[index].vertexC - inputTriangles[index].vertexA;
        Vector3 v2 = P - inputTriangles[index].vertexA;

        float d00 = Vector3.Dot(v0, v0);
        float d01 = Vector3.Dot(v0, v1);
        float d11 = Vector3.Dot(v1, v1);
        float d20 = Vector3.Dot(v2, v0);
        float d21 = Vector3.Dot(v2, v1);
        float invDenominator = 1 / (d00 * d11 - d01 * d01);
        result.y = (d11 * d20 - d01 * d21) * invDenominator;
        result.z = (d00 * d21 - d01 * d20) * invDenominator;
        result.x = 1 - result.y - result.z;
        return result;
    }

    Vector3 ProjectedPoint(Vector3 P, int index)
    {
        Vector3 normal = NormalSquared(index);
        return P - normal * (Vector3.Dot(normal, P - inputTriangles[index].vertexA) / Vector3.Dot(normal, normal));
    }

    Vector3 ClosestPointOnSegment(Vector3 projectedPoint, Vector3 A, Vector3 B)
    {
        Vector3 v = B - A;
        Vector3 w = projectedPoint - A;
        float t = Mathf.Clamp(Vector3.Dot(v, w) / Vector3.Dot(v, v), 0, 1);
        return A + v * t;
    }

    Vector3 ClosestPointOnTriangle(Vector3 projectedPoint, int index, Vector3 barMultipliers)
    {
        Vector3 result = projectedPoint;
        if (barMultipliers.x < 0)
        {
            result = ClosestPointOnSegment(projectedPoint, inputTriangles[index].vertexB, inputTriangles[index].vertexC);
        }
        if (barMultipliers.y < 0)
        {
            result = ClosestPointOnSegment(projectedPoint, inputTriangles[index].vertexA, inputTriangles[index].vertexC);
        }
        if (barMultipliers.z < 0)
        {
            result = ClosestPointOnSegment(projectedPoint, inputTriangles[index].vertexA, inputTriangles[index].vertexB);
        }
        return result;
    }

    Vector3 NormalSquared(int index)
    {
        return Vector3.Cross(inputTriangles[index].vertexB - inputTriangles[index].vertexA, inputTriangles[index].vertexC - inputTriangles[index].vertexA);
    }
    Vector3 Normal(int index)
    {
        return Vector3.Cross(inputTriangles[index].vertexB - inputTriangles[index].vertexA, inputTriangles[index].vertexC - inputTriangles[index].vertexA).normalized;
    }


    float SegmentDistance(Vector3 projectedPoint, Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 v = B - A;
        float len = Vector3.Dot(v, v);
        Vector3 w = projectedPoint - A;
        float t = Mathf.Clamp(Vector3.Dot(v, w) / Mathf.Sqrt(len), 0, 1);
        Vector3 temp = A + v * t;
        return Vector3.Dot(temp - P, temp - P);
    }

    float SquaredDistanceToTriangle(Vector3 P, int index, float multiplier)
    {
        Vector3 projectedPoint = ProjectedPoint(P, index);
        Vector3 barMultipliers = Barycentric(projectedPoint, index);

        if (barMultipliers.x >= 0 && barMultipliers.y >= 0 && barMultipliers.z >= 0) return Vector3.Dot(projectedPoint - P, projectedPoint - P);

        if (barMultipliers.x < 0)
        {
            return SegmentDistance(projectedPoint, inputTriangles[index].vertexB, inputTriangles[index].vertexC, P);
        }
        if (barMultipliers.y < 0)
        {
            return SegmentDistance(projectedPoint, inputTriangles[index].vertexA, inputTriangles[index].vertexC, P);
        }
        if (barMultipliers.z < 0)
        {
            return SegmentDistance(projectedPoint, inputTriangles[index].vertexA, inputTriangles[index].vertexB, P);
        }
        return 1;
    }

    public void CalculateDistance2(Vector3Int id)
    {
        Vector3 currentPosition = realPosExternal(id);
        float val = size * size;
        float multiplier = -10;
        for (int j = 0; j < trianglesCount && val != 0; j++)
        {
            float tempMul = Vector3.Dot(currentPosition - inputTriangles[j].vertexA, Normal(j));
            float dist = SquaredDistanceToTriangle(currentPosition, j, tempMul);
            if (dist <= val)
            {
                val = dist;
                if (tempMul != 0)
                {
                    multiplier = tempMul;
                }
            }
        }
        val = Mathf.Min(val, 1);
        if (multiplier > 0) val = -val;

        sdf[coords(id)] = val;
    }

    void CalculateDistance(Vector3Int id)
    {
        Vector3 currentPosition = realPosExternal(id);
        float val = size * size;
        Vector3 closestPoint = new Vector3(0, 0, 0);

        Vector3 t_projectedPoint = new Vector3(0, 0, 0);
        Vector3 t_vector3 = new Vector3(0, 0, 0);
        Vector3 t_closestPoint = new Vector3(0, 0, 0);
        int closestIndex = -1;
        float t_val = val;

        for (int j = 0; j < trianglesCount && val != 0; j++)
        {
            t_projectedPoint = ProjectedPoint(currentPosition, j);
            t_vector3 = Barycentric(t_projectedPoint, j);
            t_closestPoint = ClosestPointOnTriangle(t_projectedPoint, j, t_vector3);
            t_vector3 = currentPosition - t_closestPoint;
            t_val = Vector3.Dot(t_vector3, t_vector3);
            if (t_val < val)
            {
                val = t_val;
                closestPoint = new Vector3(t_closestPoint.x, t_closestPoint.y, t_closestPoint.z);
                closestIndex = j;
            }
        }
        val = Mathf.Min(val, maxValue);
        t_vector3 = Barycentric(closestPoint, closestIndex);
        if (Vector3.Dot(currentPosition - closestPoint, t_vector3.x * inputTriangles[closestIndex].normA + t_vector3.y * inputTriangles[closestIndex].normB + t_vector3.z * inputTriangles[closestIndex].normC) < 0)
        {
            val = -val;
        }
        /*
        if (currentPosition.x >= 0.8f && currentPosition.y >= 0.8f && currentPosition.z >= 0.8f && currentPosition.x <= 1.4f && currentPosition.y <= 1.4f && currentPosition.z <= 1.4f)
        {
            val = -val;
        }*/

        sdf[coords(id)] = val;
    }

    public void Compute()
    {
        for (int i = 0; i < numPointsPerAxis; i++)
        {
            for (int j = 0; j < numPointsPerAxis; j++)
            {
                for (int k = 0; k < numPointsPerAxis; k++)
                {
                    CalculateDistance(new Vector3Int(i, j, k));
                }
            }
        }

    }
}
