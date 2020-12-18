using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
public class ObjTranslator : MonoBehaviour
{
    [HideInInspector]
    public Mesh mesh;
    public float size; 
    public int resolution;
    public int chunkResolution;
    public float relativeMeshSize;
    public void ParseFileToMesh(string filename)
    {
        List<Vector3> vertices=new List<Vector3>();
        List<int>triangles=new List<int>();
        mesh=new Mesh();
        StreamReader sr=new StreamReader(filename);
        int offset=0;
        while(!sr.EndOfStream)
        {
            
            string current=sr.ReadLine();
            while(!sr.EndOfStream&&(current.Length<2 || !(current[0]=='v' && current[1]==' ')))current=sr.ReadLine();
            
            while(!sr.EndOfStream&&current.Length>1&&current[0]=='v'&& current[1]==' ')
            {
                vertices.Add(StringToVertex(current));
                current=sr.ReadLine();
            }
            while(!sr.EndOfStream&&(current.Length<2 || !(current[0]=='f' && current[1]==' ')))current=sr.ReadLine();
            while(!sr.EndOfStream&&current.Length>1&&current[0]=='f'&& current[1]==' ')
            {
                triangles.AddRange(StringToFace(current,offset));
                current=sr.ReadLine();
            }
        }
        sr.Close();

        Vector3 min=Vector3.positiveInfinity, max=Vector3.negativeInfinity;
        {
            for(int i=0;i<vertices.Count;i++)
            {
                min=Vector3.Min(min, vertices[i]);
                max=Vector3.Max(max, vertices[i]);
            }
        }
        Vector3 diff=max-min;
        float meshDiameter=Mathf.Max(diff.x, diff.y, diff.z);
        if(meshDiameter<=relativeMeshSize*size) return;
        else
        {
            float scale=(relativeMeshSize*size)/meshDiameter;
            for(int i=0;i<vertices.Count;i++) 
            {
                vertices[i]-=min;
                vertices[i]*=scale;
            }
        }
        mesh.vertices=vertices.ToArray();
        mesh.triangles=triangles.ToArray();

        Debug.Log(vertices.Count);
        Debug.Log(triangles.Count);
    }

    private Vector3 StringToVertex(string text)
    {
        float f1,f2,f3;
        int begin=2,count=0;
        string tmp;

        while(text[begin]==' ')begin++;
        while(text[begin+count]!=' ')count++;
        tmp=text.Substring(begin,count);
        float.TryParse(tmp,NumberStyles.Float, CultureInfo.InvariantCulture, out f1);

        begin+=count;
        count=0;
        while(text[begin]==' ')begin++;
        while(text[begin+count]!=' ')count++;
        tmp=text.Substring(begin,count);
        float.TryParse(tmp,NumberStyles.Float, CultureInfo.InvariantCulture,out f2);

        begin+=count;
        count=0;
        while(text[begin]==' ')begin++;
        while(begin+count<text.Length&& text[begin+count]!=' ')count++;
        tmp=text.Substring(begin,count);
        float.TryParse(tmp,NumberStyles.Float, CultureInfo.InvariantCulture,out f3);

        return new Vector3(f1,f2,f3);
    }
    private List<int> StringToFace(string text, int offset)
    {
        List<int>result=new List<int>();
        int tmp;
        int begin=0,count=0;
        while(begin+count<text.Length)
        {
            begin+=count;
            count=0;
            while(begin<text.Length&& text[begin]!=' ')begin++;
            while(begin<text.Length&&text[begin]==' ')begin++;
            while(begin+count<text.Length&&text[begin+count]!='/')count++;
            string sub=text.Substring(begin,count);
            if(sub=="")break;
            int.TryParse(sub,out tmp);
            //indices from 0, not 1
            tmp=tmp+offset-1;
            if(result.Count<3) result.Add(tmp);
            else
            {
                result.Add(result[0]);
                result.Add(result[result.Count-2]);
                result.Add(tmp);
            }
        }
        return result;
    }
}
