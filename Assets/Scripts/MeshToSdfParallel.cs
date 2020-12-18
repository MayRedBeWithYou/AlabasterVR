using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
public class MeshToSdfParallel : MonoBehaviour
{
    private int numPointsPerAxis;
    private int resolution;
    private int chunkResolution;

    private float size;
    private Mesh mesh2;
    private Vector3[] planeNormals;
    private float[] planeOffsets;
    private float[] planeDotProduct00;
    private float[] planeDotProduct01;
    private float[] planeDotProduct11;
    private float[] denominators;

    public float[] TranslateMeshToSdf()
    {
        Debug.Log("Prepared");
        var script=gameObject.GetComponent<ObjTranslator>();
        mesh2=script.mesh;
        resolution=script.resolution;
        chunkResolution=script.chunkResolution;
        numPointsPerAxis=chunkResolution*resolution+1;
        size=script.size;
        NativeArray<float> values=new NativeArray<float>(resolution*resolution*resolution*chunkResolution, Allocator.Persistent);
        for (int i=0;i<values.Length;i++)values[i]=1f;
        InitiateNormalsAndOffsets();
        InitiatePlaneDotProductsAndDenominators();
        CalculateDistanceJob job=new CalculateDistanceJob()
        {
            size=size,
            numPointsPerAxis=numPointsPerAxis,
            values=values,
            mesh=new MeshStruct()
            {
                vertices=new NativeArray<Vector3>(mesh2.vertices,Allocator.Persistent),
                triangles=new NativeArray<int>(mesh2.triangles,Allocator.Persistent),
                min=mesh2.bounds.min,
                max=mesh2.bounds.max
            },
            planeNormals=new NativeArray<Vector3>(planeNormals, Allocator.Persistent),
            planeOffsets=new NativeArray<float>(planeOffsets, Allocator.Persistent),
            planeDotProduct00=new NativeArray<float>(planeDotProduct00, Allocator.Persistent),
            planeDotProduct01=new NativeArray<float>(planeDotProduct01, Allocator.Persistent),
            planeDotProduct11=new NativeArray<float>(planeDotProduct11, Allocator.Persistent),
            denominators=new NativeArray<float>(denominators, Allocator.Persistent)
        };
        JobHandle jobHandle = job.Schedule(values.Length, 64);
        jobHandle.Complete();
        
        Debug.Log("Mesh translated");
        float[] result=values.ToArray();
        values.Dispose();
        job.planeDotProduct00.Dispose();
        job.planeDotProduct01.Dispose();
        job.planeDotProduct11.Dispose();
        job.planeNormals.Dispose();
        job.planeOffsets.Dispose();
        job.denominators.Dispose();
        job.mesh.triangles.Dispose();
        job.mesh.vertices.Dispose();
        return result;
    }

    struct MeshStruct
    {
        [ReadOnly]
        public NativeArray<Vector3> vertices;
        [ReadOnly]
        public NativeArray<int> triangles;
        [ReadOnly]
        public Vector3 min;
        [ReadOnly]
        public Vector3 max;
    }
    struct CalculateDistanceJob : IJobParallelFor
    {
        public NativeArray<float> values;
        private Vector3 currentPosition;
        [ReadOnly]
        public NativeArray<Vector3> planeNormals;
        [ReadOnly]
        public NativeArray<float> planeOffsets;
        [ReadOnly]
        public NativeArray<float> planeDotProduct00;
        [ReadOnly]
        public NativeArray<float> planeDotProduct01;
        [ReadOnly]
        public NativeArray<float> planeDotProduct11;
        [ReadOnly]
        public NativeArray<float> denominators;
        [ReadOnly]
        public float size;
        [ReadOnly]
        public int numPointsPerAxis;
        [ReadOnly]
        public int resolution;
        [ReadOnly]
        public int chunkResolution;
        [ReadOnly]
        public MeshStruct mesh;

        public void Execute(int i)
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
                        closestTriangleIndex=j;
                    }
                }
                if(values[i]>1f) values[i]=1f;
                if(values[i]!=1f) values[i]=Mathf.Sqrt(values[i]);
                if(closestTriangleIndex==-1)Debug.Log(values[i]);
                else
                {
                    float multiplier=Vector3.Dot(planeNormals[closestTriangleIndex],currentPosition)+planeOffsets[closestTriangleIndex];
                    if(multiplier>0)values[i]=-values[i];
                }
            }
        }
        private Vector3 IndexToPositions(int index)
        {
            int indZ=index/(numPointsPerAxis*numPointsPerAxis);
            int indY=(index-indZ*numPointsPerAxis*numPointsPerAxis)/(numPointsPerAxis);
            int indX=index-indZ*numPointsPerAxis*numPointsPerAxis-indY*numPointsPerAxis;
            float diff=size/(numPointsPerAxis-1);
            return new Vector3(indX*diff,indY*diff,indZ*diff );
        }
        
        private bool InBounds(Vector3 current)
        {
            return current.x+1>mesh.min.x && current.y+1>mesh.min.y && current.z+1>mesh.min.z
                    && current.x-1<mesh.max.x && current.y-1<mesh.max.y && current.z-1<mesh.max.z;
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
    private Vector3 Normal(Vector3 A,Vector3 B, Vector3 C)
    {
        //for counter-clockwiese->inside
        return Vector3.Cross(B-A,C-A);
    }

    private void InitiateNormalsAndOffsets()
    {
        planeNormals=new Vector3[mesh2.triangles.Length/3];
        planeOffsets=new float[planeNormals.Length];
        for (int i = 0; i < planeNormals.Length; i++)
        {
            planeNormals[i]=Vector3.Normalize(Normal(mesh2.vertices[mesh2.triangles[3*i]],mesh2.vertices[mesh2.triangles[3*i+1]],mesh2.vertices[mesh2.triangles[3*i+2]]));
            planeOffsets[i]=-Vector3.Dot(planeNormals[i],mesh2.vertices[mesh2.triangles[3*i]]);
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
            Vector3 v0 =  mesh2.vertices[mesh2.triangles[3*i+1]]-mesh2.vertices[mesh2.triangles[3*i]];
            Vector3 v1 =  mesh2.vertices[mesh2.triangles[3*i+2]]-mesh2.vertices[mesh2.triangles[3*i]];

            planeDotProduct00[i]=Vector3.Dot(v0, v0);
            planeDotProduct01[i]=Vector3.Dot(v0, v1);
            planeDotProduct11[i]=Vector3.Dot(v1, v1);
            denominators[i]=planeDotProduct00[i] * planeDotProduct11[i] - planeDotProduct01[i] * planeDotProduct01[i];
        }
    }
}
