using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class MeshToSdfGpu
{
    
    private static ComputeShader meshToSdfShader;
    public static void Initialize()
    {
        meshToSdfShader = Resources.Load<ComputeShader>("meshToSdf");
    }
    private static bool InBounds(Vector3 current, Vector3 min, Vector3 max)
    {
        return current.x>=min.x && current.y>=min.y && current.z>=min.z
                && current.x<=max.x && current.y<=max.y && current.z<=max.z;
    }
    public static void CreateSdf(ComputeBuffer triangles, Chunk chunk, int trianglesCount,MeshToSdfGpuCpu test)
    {
        float[] arr=new float[chunk.resolution*chunk.resolution*chunk.resolution];
        int kernel=meshToSdfShader.FindKernel("CalculateDistance");
        meshToSdfShader.SetInt("numPointsPerAxis", LayerManager.Instance.ChunkResolution+1);
        meshToSdfShader.SetInt("trianglesCount", trianglesCount);
        meshToSdfShader.SetFloat("chunkSize",LayerManager.Instance.Spacing);
        meshToSdfShader.SetFloat("size",LayerManager.Instance.Size);
        meshToSdfShader.SetVector("chunkOffset",new Vector3(chunk.coord.x * LayerManager.Instance.Spacing, chunk.coord.y * LayerManager.Instance.Spacing,chunk.coord.z * LayerManager.Instance.Spacing));
        meshToSdfShader.SetBuffer(kernel, "inputTriangles", triangles);
        meshToSdfShader.SetBuffer(kernel,"sdf",chunk.voxels.VoxelBuffer);
        meshToSdfShader.Dispatch(kernel, chunk.voxels.Resolution/8,chunk.voxels.Resolution/8,chunk.voxels.Resolution/8);
        chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
        
        chunk.voxels.VoxelBuffer.GetData(arr);
        string s="";
        s=string.Join(" ",arr);
        StreamWriter stream=new StreamWriter(chunk.name+".txt");
        stream.Write(s);
        stream.Close();
        
        for (int i = 0; i < chunk.voxels.Resolution; i++)
        {
            for (int j = 0; j < chunk.voxels.Resolution; j++)
            {
                for (int k = 0; k < chunk.voxels.Resolution; k++)
                {
                    test.CalculateDistance(new Vector3Int(i,j,k));
                }
            }
        }
        s="";
        s=string.Join(" ",test.sdf);
        stream=new StreamWriter("_CPU"+chunk.name+".txt");
        stream.Write(s);
        stream.Close();
        chunk.InitFromMesh(test.sdf);
        chunk.gpuMesh.UpdateVertexBuffer(chunk.voxels);
    }
    private static Vector3 Normal(Vector3 A,Vector3 B, Vector3 C)
    {
        //for counter-clockwiese->inside
        return Vector3.Cross(B-A,C-A);
    }
    public static ComputeBuffer CreateTrianglesBuffer(Mesh mesh)
    { 
        int trianglesCount=mesh.triangles.Length/3;
        ObjTriangle[] trianglesArray=new ObjTriangle[trianglesCount];
        ComputeBuffer result=new ComputeBuffer(trianglesCount*12,sizeof(float));
        
        for (int i=0;i<trianglesCount;i++)
        {
            ObjTriangle tmp;
            tmp.vertexA=mesh.vertices[mesh.triangles[3*i]];
            tmp.vertexB=mesh.vertices[mesh.triangles[3*i+1]];
            tmp.vertexC=mesh.vertices[mesh.triangles[3*i+2]];
            tmp.normal=Vector3.Normalize(Normal(tmp.vertexA,tmp.vertexB,tmp.vertexC));

            trianglesArray[i]=tmp;
        }
        result.SetData(trianglesArray);
                
        return result;
    }
}
public struct ObjTriangle{
    public Vector3 vertexC;
    public Vector3 vertexA;
    public Vector3 vertexB;
    public Vector3 normal;

    public override string ToString()
    {
        return " C:"+vertexC.ToString()+"-A:"+vertexA.ToString()+ "-B:"+vertexB.ToString()+"-n:"+normal.ToString();
    }
    };
