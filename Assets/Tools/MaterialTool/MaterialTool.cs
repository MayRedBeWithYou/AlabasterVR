using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MaterialTool : Tool
{

    [Header("Handlers")]

    public AxisHandler trigger;
    public ButtonHandler settingsButton;
    public ButtonHandler toggleButton;
    public ButtonHandler positionButton;

    public ButtonHandler upButton;
    public ButtonHandler downButton;

    [Header("Cursor")]
    public Material addMaterial;
    public Material removeMaterial;

    [Space(10f)]
    public GameObject cursorPrefab;

    [HideInInspector]
    public CursorSDF cursor;

    public bool isAdding = true;

    public override void Enable()
    {

        if (cursor != null)
            cursor.ToggleRenderer(true);
        base.Enable();
    }

    private void PositionButtonHandler(object sender, bool value)
    {
        if(value)
        {
            cursor.transform.parent = null;
        }
        else
        {
            cursor.transform.parent = ToolController.Instance.rightController.transform;
        }
    }

    public override void Disable()
    {
        cursor.ToggleRenderer(false);
        base.Disable();
    }

    public void Awake()
    {
        cursor = Instantiate(cursorPrefab, ToolController.Instance.rightController.transform).GetComponent<CursorSDF>();
        toggleButton.StateChanged += ToggleButtonHandler;
        positionButton.StateChanged += PositionButtonHandler;
    }

    private void FixedUpdate()
    {
        cursor.UpdateActiveChunks();
        if (upButton.Value)
        {
            cursor.IncreaseRadius();
        }
        if (downButton.Value)
        {
            cursor.DecreaseRadius();
        }
        if(trigger.Value > 0.2)
        {
            cursor.UpdateActiveChunks();

            PerformAction();
        }
    }

    private void ToggleButtonHandler(object sender, bool value)
    {
        isAdding = !isAdding;
        cursor.SetMaterial(isAdding ? addMaterial : removeMaterial);
    }

    private void PerformAction()
    {
        foreach (Chunk chunk in LayerManager.Instance.activeChunks)
        {
            for (int x = 0; x < chunk.size; x++)
            {
                for (int y = 0; y < chunk.size; y++)
                {
                    for (int z = 0; z < chunk.size; z++)
                    {
                        int index = chunk.GetVoxelIndex(x, y, z);
                        float val = cursor.IsInside(chunk.voxelArray[index].position);
                        if (isAdding)
                        {
                            chunk.voxelArray[index].value = Mathf.Min(val, chunk.voxelArray[index].value);
                        }
                        else
                        {
                            chunk.voxelArray[index].value = Mathf.Max(-val, chunk.voxelArray[index].value);
                        }
                    }
                }
            }
        }

        MeshGenerator.Instance.UpdateAllActiveChunks();
    }
}
