using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public class MeshToSdfGpuCpu
{
    const int numThreads = 8;

public float[] sdf;
public ObjTriangle[]  inputTriangles;
int numPointsPerAxis;
int trianglesCount;
float chunkSize;
float size;
public Vector3 chunkOffset;

    public MeshToSdfGpuCpu(float[] sdf, ObjTriangle[] inputTriangles, int numPointsPerAxis, int trianglesCount, float chunkSize, float size, Vector3 chunkOffset)
    {
        this.sdf = sdf;
        this.inputTriangles = inputTriangles;
        this.numPointsPerAxis = numPointsPerAxis;
        this.trianglesCount = trianglesCount;
        this.chunkSize = chunkSize;
        this.size = size;
        this.chunkOffset = chunkOffset;
    }

    int coords(Vector3Int id)
{
    return id.z * (numPointsPerAxis-1) * (numPointsPerAxis-1) + id.y * (numPointsPerAxis-1) + id.x;
}

Vector3 realPosExternal(Vector3Int id)
{
    float res = numPointsPerAxis - 1;
    Vector3 ids = (Vector3)id / res;
    return (ids * chunkSize)+chunkOffset;
}
Vector3 Barycentric(Vector3 P, int index)
{
    //https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates

    Vector3 result;
    Vector3 v0 =  inputTriangles[index].vertexB - inputTriangles[index].vertexA;
    Vector3 v1 =  inputTriangles[index].vertexC - inputTriangles[index].vertexA;
    Vector3 v2 =  P - inputTriangles[index].vertexA;
    
    float d00 = Vector3.Dot(v0, v0);
    float d01 = Vector3.Dot(v0, v1);
    float d11 = Vector3.Dot(v1, v1);
    float d20 = Vector3.Dot(v2, v0);
    float d21 = Vector3.Dot(v2, v1);
    float denominator = d00 * d11 - d01 * d01;
    result.y = (d11 * d20 - d01 * d21) / denominator;
    result.z = (d00 * d21 - d01 * d20) / denominator;
    result.x = 1 - result.y - result.z;
    return result;
}
float SegmentDistance(Vector3 projectedPoint, Vector3 A, Vector3 B, Vector3 P)
{
    Vector3 v=B-A;
    float len=Vector3.Dot(v,v);
    Vector3 w=projectedPoint-A;
    float t=Mathf.Clamp(Vector3.Dot(v,w)/len,0,1);
    Vector3 temp=A+v*t;
    return Vector3.Dot(temp-P,temp-P);
}

float SquaredDistanceToTriangle(Vector3 P, int index, float multiplier)
{
    Vector3 projectedPoint=P-(multiplier*inputTriangles[index].normal);
    Vector3 barMultipliers= Barycentric(projectedPoint, index);
    
    if(barMultipliers.x>=0 && barMultipliers.y>=0 && barMultipliers.z>=0) return Vector3.Dot(projectedPoint-P,projectedPoint-P);

    if(barMultipliers.x<0) 
    {
        return SegmentDistance(projectedPoint,inputTriangles[index].vertexB,inputTriangles[index].vertexC, P);
    }
    if(barMultipliers.y<0) 
    {
        return SegmentDistance(projectedPoint,inputTriangles[index].vertexA,inputTriangles[index].vertexC, P);
    }
    if(barMultipliers.z<0) 
    {
        return SegmentDistance(projectedPoint,inputTriangles[index].vertexA,inputTriangles[index].vertexB, P);
    }
    return 1;
}


public void CalculateDistance(Vector3Int id)
{
    Vector3 currentPosition=realPosExternal(id);
    float val=size*size;
    float multiplier=-10;
    for (int j = 0; j < trianglesCount && val!=0; j++)
    {
        float tempMul=Vector3.Dot(currentPosition - inputTriangles[j].vertexA, inputTriangles[j].normal);
        float dist=SquaredDistanceToTriangle(currentPosition, j, tempMul);
        if(dist<=val)
        {
            val=dist;
            if(tempMul!=0)
            {
                multiplier=tempMul;
            }
        }
    }
    val=Mathf.Min(val,1);
    if(multiplier<0)val=-val;

    sdf[coords(id)]=val;
}
}


