using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMaterialCursor : MaterialCursor
{
    private CylindricalCursor cursor;

    private int addMaterialKernel;
    private int removeMaterialKernel;

    public ComputeShader shader;

    public override int AddMaterialKernel { get => addMaterialKernel; }
    public override int RemoveMaterialKernel { get => removeMaterialKernel; }

    public override ComputeShader PrepareShader(Color color)
    {
        shader.SetFloat("radius", cursor.Size);
        shader.SetVector("color", new Vector3(color.r, color.g, color.b));
        return shader;
    }

    public override void UpdateActiveChunks()
    {
        cursor.UpdateActiveChunks();
    }

    protected void Awake()
    {
        cursor = GetComponent<CylindricalCursor>();
        InitializeShadersConstUniforms();
    }

    private void InitializeShadersConstUniforms()
    {
        var activeLayer = LayerManager.Instance.ActiveLayer;

        shader.SetFloat("chunkSize", activeLayer.Spacing);
        shader.SetInt("resolution", activeLayer.ChunkResolution);
        shader.SetFloat("voxelSpacing", LayerManager.Instance.VoxelSpacing);
        shader.SetFloat("radius", cursor.Size);
    }

    void Start()
    {
        addMaterialKernel = shader.FindKernel("AddMaterial");
        removeMaterialKernel = shader.FindKernel("RemoveMaterial");
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }

    public override void SetColor(Color color)
    {
        cursor.Color = color;
    }

    public override void IncreaseSize()
    {
        cursor.IncreaseSize();
    }

    public override void DecreaseSize()
    {
        cursor.DecreaseSize();
    }

    public override void SetMaximumSize(float value)
    {
        cursor.MaximalSize = value;
    }

    public override void SetMinimumSize(float value)
    {
        cursor.MinimalSize = value;
    }

    public override void SetSizeToDefault()
    {
        cursor.SetSizeToDefault();        
    }
}
