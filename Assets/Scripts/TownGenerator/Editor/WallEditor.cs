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

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        SerializedProperty columns = data.FindPropertyRelative("m_Columns");
        SerializedProperty rows = data.FindPropertyRelative("m_Rows");
        SerializedProperty material = data.FindPropertyRelative("m_Material");

        EditorGUILayout.LabelField("Wall Section Layout");
        EditorGUILayout.PropertyField(columns);
        EditorGUILayout.PropertyField(rows);
        EditorGUILayout.PropertyField(material);


        if (serializedObject.ApplyModifiedProperties())
        {
            wall.Build();
        }
    }
}
