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
        SerializedProperty scale = serializedObject.FindProperty("m_FrameScale");

        EditorGUILayout.PropertyField(cols);
        EditorGUILayout.PropertyField(rows);
        EditorGUILayout.LabelField("Size");
        float y = EditorGUILayout.Slider("Height", scale.vector3Value.y, 0, 1);
        float x = EditorGUILayout.Slider("Width", scale.vector3Value.x, 0, 1);
        scale.vector3Value = new Vector3(x, y, scale.vector3Value.z);

        if (serializedObject.ApplyModifiedProperties())
        {
            window.Build();
        }
    }
}
