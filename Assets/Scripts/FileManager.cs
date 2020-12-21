using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FileManager : MonoBehaviour
{
    private static FileManager _util;
    public static FileManager Util { get { return _util; } }

    public GameObject FileExplorerPrefab;
    private GameObject _fileExplorer;
    private float dist;
    private string path;
    void Awake()
    {
        if (_util != null && _util != this) Destroy(this.gameObject);
        else _util = this;
        var script=GameObject.Find("LayerManager").GetComponent<LayerManager>();
        size=script.Size;
        resolution=script.Resolution;
        chunkResolution=script.ChunkResolution;
        dist=GameObject.Find("ToolController").GetComponent<ToolController>().uiDistance;
    }
    private float size; 
    private int resolution;
    private int chunkResolution;
    
    public GameObject SaveModel()
    {
        path="";
        if(_fileExplorer!=null)
        {
            Destroy(_fileExplorer);
        }
        else
        {
            var script=PrepareGeneral();
            script.mode=FileExplorerMode.Save;
            script.UpdateDirectory();
            script.OnAccepted+=(text)=>
            {
                if(System.String.IsNullOrWhiteSpace(text)) return;
                path=text+".obj";
                script.Close();
            };
            
            if(!System.String.IsNullOrWhiteSpace(path))
            {
                TranslateModelToObj(path);
                //Debug.Log(path);
            }
        }
        return _fileExplorer;
    }

    public GameObject LoadModel()
    {
        path="";
        if(_fileExplorer!=null)
        {
            Destroy(_fileExplorer);
        }
        else
        {
            var script=PrepareGeneral();
            script.mode=FileExplorerMode.Open;
            script.SetExtensionsArray(new string[]{".obj"});
            script.UpdateDirectory();
            script.OnAccepted+=(text)=>
            {
                if(System.String.IsNullOrWhiteSpace(text)) return;
                path=text;
                script.Close();
            };
            //todo:meshtosdf+fix filename error
            //Debug.Log(path);
        }
        return _fileExplorer;
    }
    public GameObject LoadImageReference()
    {
        path="";
        if(_fileExplorer!=null)
        {
            Destroy(_fileExplorer);
        }
        else
        {
            var script=PrepareGeneral();
            script.mode=FileExplorerMode.Open;
            script.SetExtensionsArray(new string[]{".jpg",".png"});
            script.UpdateDirectory();
            script.OnAccepted+=(text)=>
            {
                if(System.String.IsNullOrWhiteSpace(text)) return;
                path=text;
                script.Close();
            };
        }
        
        
        //todo: prepare prefab and put image into it
        return _fileExplorer;
    }

    public void CloseFileExplorer()
    {
        if(_fileExplorer!=null) 
        {
            _fileExplorer.GetComponent<FileExplorer>().Close();
        }
    }
 
    private FileExplorer PrepareGeneral()
    {
        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;
        Vector3 prefabPosition=Camera.main.transform.position + lookDirection.normalized * (dist+0.1f);
        _fileExplorer=Instantiate(FileExplorerPrefab, prefabPosition, Quaternion.LookRotation(lookDirection, Vector3.up));
        var script=_fileExplorer.GetComponent<FileExplorer>();
        script.OnCancelled+=()=>script.Close();
        return script;
    }
    private void TranslateModelToObj(string path)
    {
        
    }
}
