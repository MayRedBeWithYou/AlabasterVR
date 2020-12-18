using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MeshToSdf : MonoBehaviour
{
    public float [] values;
    private Vector3[] planeNormals;
    private float[] planeOffsets;
    private float[] planeDotProduct00;
    private float[] planeDotProduct01;
    private float[] planeDotProduct11;
    private float[] denominators;
    private float size;
    private int resolution;
    private int chunkResolution;
    private float diff;
    private Mesh mesh;
    
    
    public float[] TranslateMeshToSdf()
    {
        var script=gameObject.GetComponent<ObjTranslator>();
        mesh=script.mesh;
        size=script.size;
        resolution=script.resolution;
        chunkResolution=script.chunkResolution;
        diff=size/(resolution*chunkResolution);

        values=new float[resolution*resolution*resolution*(chunkResolution+1)];
        values=Enumerable.Repeat(1f, values.Length).ToArray();

        InitiateNormalsAndOffsets();
        InitiatePlaneDotProductsAndDenominators();
        Debug.Log("Prepared");
        CalculateDistances();
        Debug.Log("Mesh translated");
        return values;
    }
    
    Vector3Int idToGridCoords(int id)
    {
        Vector3Int coords= Vector3Int.zero;
        int resSq = chunkResolution * chunkResolution;
        coords.z = id / resSq;
        id -= coords.z * resSq;
        coords.y = id / chunkResolution;
        coords.x = id % chunkResolution;
        return coords;
    }
    private Vector3 IndexToPositions(int index)
    {
        int res3=resolution*resolution*resolution;
        int res2=resolution*resolution;
        
        int chunkX=index/res3;
        index-=chunkX*res3;
        int chunkY=index/(res2);
        index-=chunkY*res2;
        int chunkZ=index/resolution;
        index-=chunkZ*resolution;
        Vector3 outside=new Vector3(chunkX,chunkY,chunkZ);
        Vector3 inside=idToGridCoords(index);
        return ((outside*chunkResolution)+inside)*diff;
    }

    private void InitiateNormalsAndOffsets()
    {
        planeNormals=new Vector3[mesh.triangles.Length/3];
        planeOffsets=new float[planeNormals.Length];
        for (int i = 0; i < planeNormals.Length; i++)
        {
            planeNormals[i]=Vector3.Normalize(Normal(mesh.vertices[mesh.triangles[3*i]],mesh.vertices[mesh.triangles[3*i+1]],mesh.vertices[mesh.triangles[3*i+2]]));
            planeOffsets[i]=-Vector3.Dot(planeNormals[i],mesh.vertices[mesh.triangles[3*i]]);
        }
    }
    private void InitiatePlaneDotProductsAndDenominators()
    {
        planeDotProduct00=new float[planeNormals.Length];
        planeDotProduct01=new float[planeNormals.Length];
        planeDotProduct11=new float[planeNormals.Length];
        denominators=new float[planeNormals.Length];
        for (int i = 0; i < planeDotProduct00.Length; i++)
        {
            Vector3 v0 =  mesh.vertices[mesh.triangles[3*i+1]]-mesh.vertices[mesh.triangles[3*i]];
            Vector3 v1 =  mesh.vertices[mesh.triangles[3*i+2]]-mesh.vertices[mesh.triangles[3*i]];

            planeDotProduct00[i]=Vector3.Dot(v0, v0);
            planeDotProduct01[i]=Vector3.Dot(v0, v1);
            planeDotProduct11[i]=Vector3.Dot(v1, v1);
            denominators[i]=planeDotProduct00[i] * planeDotProduct11[i] - planeDotProduct01[i] * planeDotProduct01[i];
        }
    }
    private Vector3 Normal(Vector3 A,Vector3 B, Vector3 C)
    {
        return Vector3.Cross(B-A,C-A);
    }
    private bool InBounds(Vector3 current)
    {
        return current.x+1>mesh.bounds.min.x && current.y+1>mesh.bounds.min.y && current.z+1>mesh.bounds.min.z
                && current.x-1<mesh.bounds.max.x && current.y-1<mesh.bounds.max.y && current.z-1<mesh.bounds.max.z;
    }
    private void CalculateDistances()
    {
        Vector3 currentPosition;
        for (int i = 0; i < values.Length; i++)
        {
            currentPosition=IndexToPositions(i);
            if(InBounds(currentPosition))
            {
                int closestTriangleIndex=-1;
                values[i]=size;
                for (int j = 0; j < planeNormals.Length && values[i]!=0; j++)
                {
                    float dist=SquaredDistanceToTriangle(currentPosition, j);
                    if(dist<=values[i])
                    {
                        values[i]=dist;
                        if(Vector3.Dot(currentPosition,planeNormals[j])+planeOffsets[j]!=0)closestTriangleIndex=j;
                    }
                }
                if(values[i]>1f) values[i]=1f;
                if(values[i]!=1f) values[i]=Mathf.Sqrt(values[i]);
                else
                {
                    float multiplier=Vector3.Dot(planeNormals[closestTriangleIndex],currentPosition)+planeOffsets[closestTriangleIndex];
                    if(multiplier>0)values[i]=-values[i];
                }
            }
        }
    }

    private float SquaredDistanceToTriangle(Vector3 P, int index)
    {
        float multiplier=Vector3.Dot(planeNormals[index],P)+planeOffsets[index];
        Vector3 projectedPoint=P-(multiplier*planeNormals[index]);
        float alfa, beta, gamma;
        Barycentric(projectedPoint, index, out alfa, out beta, out gamma);
        
        if(alfa>=0&&beta>=0&&gamma>=0) return multiplier*multiplier;
        if(alfa<0) return SegmentDistance(projectedPoint,mesh.vertices[mesh.triangles[3*index+1]],mesh.vertices[mesh.triangles[3*index+2]], P);
        if(beta<0) return SegmentDistance(projectedPoint,mesh.vertices[mesh.triangles[3*index]],mesh.vertices[mesh.triangles[3*index+2]], P);
        if(gamma<0) return SegmentDistance(projectedPoint,mesh.vertices[mesh.triangles[3*index]],mesh.vertices[mesh.triangles[3*index+1]], P);

        return 1f;
    }
    private void Barycentric(Vector3 P, int index, out float alfa, out float beta, out float gamma)
    {
        //https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
        Vector3 v0 =  mesh.vertices[mesh.triangles[3*index+1]]-mesh.vertices[mesh.triangles[3*index]];
        Vector3 v1 =  mesh.vertices[mesh.triangles[3*index+2]]-mesh.vertices[mesh.triangles[3*index]];
        Vector3 v2 =  P-mesh.vertices[mesh.triangles[3*index]];
        
        float d20 = Vector3.Dot(v2, v0);
        float d21 = Vector3.Dot(v2, v1);
        
        beta = (planeDotProduct11[index] * d20 - planeDotProduct01[index] * d21) / denominators[index];
        gamma = (planeDotProduct00[index] * d21 - planeDotProduct01[index] * d20) / denominators[index];
        alfa = 1.0f- beta - gamma;
    }
    private float SegmentDistance(Vector3 projectedPoint, Vector3 A, Vector3 B, Vector3 P)
    {
        //http://geomalgorithms.com/a02-_lines.html
       Vector3 v = B - A;
       Vector3 w = projectedPoint - A;

       float c1=Vector3.Dot(v,w);
       float c2=Vector3.Dot(v,v);

       if ( c1 <= 0 ) return (A-P).sqrMagnitude;  // closest A
       if ( c2 <= c1 ) return (B-P).sqrMagnitude; //closest B
       
       Vector3 temp=A+(c1/c2)*v;
       return (temp-P).sqrMagnitude;
    }
}
