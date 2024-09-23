using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGeneration))]
public class AddButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomGeneration myScript = (RoomGeneration)target;

        if (GUILayout.Button("Generate Rooms"))
        {
            myScript.Generate();
        }
    }
}
