using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FileManager : MonoBehaviour
{
    public  GameObject FileWindowPrefab;
    private GameObject _fileWindow;
    public GameObject SdfGeneratorPrefab;
    private GameObject _sdfGenerator;

    public void Initialize()
    {
        size=LayerManager.Instance.Size;
        resolution=LayerManager.Instance.Resolution;
        chunkResolution=LayerManager.Instance.ChunkResolution;
    }
    private float size; 
    private int resolution;
    private int chunkResolution;
    public void SaveToObj(bool merged)
    {

    }
    private string GetSavingPath()
    {
        string result="";
        return result;
    }
    private string GetLoadingPath()
    {
        string result="";
        //result="cube.obj";
        //result="deer.obj";
        result="Sting-Sword-lowpoly.obj";
        return result;
    }
    public void Save()
    {

    }

    public void Load()
    {
        string fileName=GetLoadingPath();
        
        _sdfGenerator=Instantiate(SdfGeneratorPrefab);
        var script=_sdfGenerator.GetComponent<ObjTranslator>();
        script.size=size;
        script.resolution=resolution;
        script.chunkResolution=chunkResolution;
        script.ParseFileToMesh(fileName);
        
        Mesh mesh=script.mesh;

        MeshToSdfGpu.Initialize();
        GPUMesh.Initialize();
        ComputeBuffer triangleBuffer= MeshToSdfGpu.CreateTrianglesBuffer(mesh);
        ObjTriangle[] trianglesArr=new ObjTriangle[mesh.triangles.Length/3];
        triangleBuffer.GetData(trianglesArr);
        MeshToSdfGpuCpu test=new MeshToSdfGpuCpu(new float[chunkResolution*chunkResolution*chunkResolution],trianglesArr,chunkResolution+1,trianglesArr.Length,LayerManager.Instance.Spacing,size,Vector3.zero);
        var Layer=LayerManager.Instance.ActiveLayer;
        LayerManager.Instance.activeChunks.Clear();
        foreach(var chunk in Layer.chunks)
        {   
            if(mesh.bounds.Intersects(chunk.ColliderBounds))
            {
                //Debug.Log("hit: "+chunk.name);
                test.sdf=new float[chunkResolution*chunkResolution*chunkResolution];
                test.chunkOffset=new Vector3(chunk.coord.x * LayerManager.Instance.Spacing, chunk.coord.y * LayerManager.Instance.Spacing,chunk.coord.z * LayerManager.Instance.Spacing);
                MeshToSdfGpu.CreateSdf(triangleBuffer,chunk,mesh.triangles.Length/3,test);
                chunk.voxels.Initialized=true;
                LayerManager.Instance.activeChunks.Add(chunk);
            }
        }
        Debug.Log("mesh done");
        triangleBuffer.Release();
        Destroy(_sdfGenerator);
    }
}
