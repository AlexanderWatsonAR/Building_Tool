using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Wall wall = (Wall)target;

        SerializedProperty columns = serializedObject.FindProperty("m_Columns");
        SerializedProperty rows = serializedObject.FindProperty("m_Rows");
        SerializedProperty material = serializedObject.FindProperty("m_Material");

        EditorGUILayout.LabelField("Wall Section Layout");
        EditorGUILayout.PropertyField(columns);
        EditorGUILayout.PropertyField(rows);
        EditorGUILayout.PropertyField(material);

        int wColumns = wall.Columns;
        int wRows = wall.Rows;

        if (serializedObject.ApplyModifiedProperties())
        {
            if (columns.intValue != wColumns ||
                rows.intValue != wRows)
            {
                wall.Build();
            }
        }
    }
}
