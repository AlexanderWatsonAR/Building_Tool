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

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        SerializedProperty columns = data.FindPropertyRelative("m_Columns");
        SerializedProperty rows = data.FindPropertyRelative("m_Rows");

        EditorGUILayout.LabelField("Roof Section Layout");
        EditorGUILayout.PropertyField(columns);
        EditorGUILayout.PropertyField(rows);

        if(serializedObject.ApplyModifiedProperties())
        {
            tile.Build();
        }
    }
}
