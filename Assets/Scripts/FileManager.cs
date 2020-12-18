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

    public void Awake()
    {
        var script=GameObject.Find("LayerManager").GetComponent<LayerManager>();
        size=script.Size;
        resolution=script.Resolution;
        chunkResolution=script.ChunkResolution;
        Load();
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
        string result="cube.obj";
        return result;
    }
    public void Save()
    {

    }

    public float[] Load()
    {
        string fileName=GetLoadingPath();
        _sdfGenerator=Instantiate(SdfGeneratorPrefab);
        var script=_sdfGenerator.GetComponent<ObjTranslator>();
        script.size=size;
        script.resolution=resolution;
        script.chunkResolution=chunkResolution;
        script.ParseFileToMesh(fileName);
        
        float[] values=null;
        //values=_sdfGenerator.GetComponent<MeshToSdf>().TranslateMeshToSdf();
        /*
        StreamWriter sw=new StreamWriter("values.log");
        for(int i=0;i<values.Length;i++)
        {
            sw.WriteLine(System.String.Format("{0} : {1}",i, values[i]));
        }
        sw.Close();
        */
        Destroy(_sdfGenerator);
        return values;
    }
}
