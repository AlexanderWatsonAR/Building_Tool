using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Building buildingScript = (Building)target;

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Build"))
        {
            //buildingScript.Construct();
            buildingScript.Construct();
        }

        if (GUILayout.Button("Reset"))
        {
            buildingScript.RevertToPolyshape();
        }

        EditorGUILayout.EndHorizontal();
    }
}
