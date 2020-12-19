using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SnapShotController : MonoBehaviour
{

    public GameObject ToFollow;
    public ComputeShader SnapshotShader;
    int resolution;
    int volume;
    float size;
    float spacing;
    ComputeBuffer data;
    ComputeBuffer overlapCounter;
    public Material pointMaterial;
    Vector3Int gridPos;

    public void SetPosition(Vector3 pos)
    {
        gridPos = LayerManager.Instance.SnapToGridPosition(pos);
        transform.position = (Vector3)gridPos * spacing;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.one * size * 0.5f, Vector3.one * size);
    }

    // Start is called before the first frame update
    void Start()
    {
        var manager = LayerManager.Instance;
        resolution = manager.ChunkResolution;
        size = manager.Size / manager.Resolution;
        spacing = size / (resolution - 1);
        volume = resolution * resolution * resolution;
        overlapCounter = new ComputeBuffer(volume, sizeof(uint));
        data = new ComputeBuffer(volume, sizeof(float));
        data.SetData(Enumerable.Repeat(1.0f, volume).ToArray());
        overlapCounter.SetData(Enumerable.Repeat(0, volume).ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        SetPosition(ToFollow.transform.position);
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            TakeSnapshot();
        }
        Draww();
    }

    void TakeSnapshot()
    {
        var overlappedColliders = Physics.OverlapBox(
            transform.position,
            Vector3.one * size * 0.5f,
            Quaternion.identity,
            1 << 9);
        var kernel = SnapshotShader.FindKernel("CSMain");
        var normalize = SnapshotShader.FindKernel("Normalize");
        SnapshotShader.SetBuffer(kernel, "snapshot", data);
        SnapshotShader.SetBuffer(kernel, "overlaps", overlapCounter);

        SnapshotShader.SetBuffer(normalize, "overlaps", overlapCounter);
        SnapshotShader.SetBuffer(normalize, "snapshot", data);
        
        foreach (Collider collider in overlappedColliders)
        {
            Chunk chunk = collider.GetComponent<Chunk>();

            Vector3 snapLocalPos = chunk.transform.worldToLocalMatrix.MultiplyPoint(transform.position);
            //Vector3Int gridOffset = LayerManager.Instance.SnapToGridPosition(snapLocalPos);
            SnapshotShader.SetBuffer(kernel, "sdf", chunk.voxels.VoxelBuffer);
            SnapshotShader.SetVector("gridDisplacement", snapLocalPos / spacing);
            SnapshotShader.SetInt("resolution", resolution);
            SnapshotShader.Dispatch(kernel, resolution / 8, resolution / 8, resolution / 8);
        }
        SnapshotShader.Dispatch(normalize, resolution / 8, resolution / 8, resolution / 8);
    }


    private void Draww()
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        materialBlock.SetBuffer("data", data);
        materialBlock.SetVector("offset", transform.position);
        materialBlock.SetFloat("spacing", spacing);
        materialBlock.SetInt("res", resolution);
        Graphics.DrawProcedural(
            pointMaterial,
            new Bounds(Vector3.zero, new Vector3(100, 100, 100)), //what exactly should go here?
            MeshTopology.Points,
            1,
            volume,
            null,
            materialBlock
            );
    }
}
