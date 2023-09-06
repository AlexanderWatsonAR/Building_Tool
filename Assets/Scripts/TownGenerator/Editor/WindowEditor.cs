using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Window))]
public class WindowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Window window = (Window)target;

        SerializedProperty cols = serializedObject.FindProperty("m_Columns");
        SerializedProperty rows = serializedObject.FindProperty("m_Rows");
        SerializedProperty scale = serializedObject.FindProperty("m_GridFrameScale");

        EditorGUILayout.PropertyField(cols);
        EditorGUILayout.PropertyField(rows);
        EditorGUILayout.PropertyField(scale);

        if (serializedObject.ApplyModifiedProperties())
        {
            window.Build();
        }
    }
}
