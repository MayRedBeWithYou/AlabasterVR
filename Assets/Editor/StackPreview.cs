using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OperationManager))]
public class StackPreview : Editor
{

    private void OnEnable()
    {
        var om = (OperationManager)target;
        om.OnStacksChanged += this.Repaint;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var bold = new GUIStyle();
        bold.fontStyle = FontStyle.Bold;
        bold.normal.textColor = Color.white;
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        GUILayout.Label("Undo", bold);
        var manager = (OperationManager)target;

        foreach (IOperation op in manager.undoOperations)
        {
            GUILayout.Label(op.GetType().Name);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Redo", bold);

        foreach (IOperation op in manager.redoOperations)
        {
            GUILayout.Label(op.GetType().Name);
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}