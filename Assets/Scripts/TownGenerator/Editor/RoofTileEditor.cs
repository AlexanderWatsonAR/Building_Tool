using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoofTile))]
public class RoofTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        RoofTile tile = (RoofTile)target;

        SerializedProperty columns = serializedObject.FindProperty("m_Columns");
        SerializedProperty rows = serializedObject.FindProperty("m_Rows");
        SerializedProperty material = serializedObject.FindProperty("m_Material");

        EditorGUILayout.LabelField("Roof Section Layout");
        EditorGUILayout.PropertyField(columns);
        EditorGUILayout.PropertyField(rows);
        EditorGUILayout.PropertyField(material);

        if(serializedObject.ApplyModifiedProperties())
        {
            tile.Build();
        }
    }
}
