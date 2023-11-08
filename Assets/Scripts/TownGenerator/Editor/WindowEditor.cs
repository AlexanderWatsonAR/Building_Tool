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
        WindowData windowData = window.WindowData;

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        SerializedProperty activeElements = data.FindPropertyRelative("m_ActiveElements");

        SerializedProperty outerScale = data.FindPropertyRelative("m_OuterFrameScale");
        SerializedProperty outerFrameDepth = data.FindPropertyRelative("m_OuterFrameDepth");

        SerializedProperty cols = data.FindPropertyRelative("m_InnerFrameColumns");
        SerializedProperty rows = data.FindPropertyRelative("m_InnerFrameRows");
        SerializedProperty innerScale = data.FindPropertyRelative("m_InnerFrameScale");
        SerializedProperty innerFrameDepth = data.FindPropertyRelative("m_InnerFrameDepth");

        SerializedProperty paneDepth = data.FindPropertyRelative("m_PaneDepth");

        SerializedProperty shuttersDepth = data.FindPropertyRelative("m_ShuttersDepth");
        SerializedProperty shuttersAngle = data.FindPropertyRelative("m_ShuttersAngle");

        EditorGUILayout.PropertyField(activeElements);

        EditorGUI.BeginDisabledGroup(!windowData.IsOuterFrameActive);
        EditorGUILayout.LabelField("Outer Frame", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(outerScale, new GUIContent("Scale"));
        EditorGUILayout.PropertyField(outerFrameDepth, new GUIContent("Depth"));
        EditorGUI.indentLevel--;
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!windowData.IsInnerFrameActive);
        EditorGUILayout.LabelField("Inner Frame", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(cols, new GUIContent("Columns"));
        EditorGUILayout.PropertyField(rows, new GUIContent("Rows"));
        EditorGUILayout.PropertyField(innerScale, new GUIContent("Scale"));
        EditorGUILayout.PropertyField(innerFrameDepth, new GUIContent("Depth"));
        EditorGUI.indentLevel--;
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!windowData.IsPaneActive);
        EditorGUILayout.LabelField("Pane", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(paneDepth, new GUIContent("Depth"));
        EditorGUI.indentLevel--;
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!windowData.AreShuttersActive);
        EditorGUILayout.LabelField("Shutters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(shuttersDepth, new GUIContent("Depth"));
        EditorGUILayout.PropertyField(shuttersAngle, new GUIContent("Angle"));
        EditorGUI.indentLevel--;
        EditorGUI.EndDisabledGroup();

        if (serializedObject.ApplyModifiedProperties())
        {
            window.Build();
        }
    }
}
