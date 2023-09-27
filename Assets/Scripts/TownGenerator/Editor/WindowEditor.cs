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
        SerializedProperty outerFrameMat = data.FindPropertyRelative("m_OuterFrameMaterial");

        SerializedProperty cols = data.FindPropertyRelative("m_Columns");
        SerializedProperty rows = data.FindPropertyRelative("m_Rows");
        SerializedProperty innerScale = data.FindPropertyRelative("m_InnerFrameScale");
        SerializedProperty innerFrameDepth = data.FindPropertyRelative("m_InnerFrameDepth");
        SerializedProperty innerFrameMat = data.FindPropertyRelative("m_InnerFrameMaterial");

        SerializedProperty paneDepth = data.FindPropertyRelative("m_PaneDepth");
        SerializedProperty paneMat = data.FindPropertyRelative("m_PaneMaterial");

        SerializedProperty shuttersDepth = data.FindPropertyRelative("m_ShuttersDepth");
        SerializedProperty shuttersAngle = data.FindPropertyRelative("m_ShuttersAngle");
        SerializedProperty shuttersMat = data.FindPropertyRelative("m_ShuttersMaterial");

        EditorGUILayout.PropertyField(activeElements);

        EditorGUI.BeginDisabledGroup(!windowData.IsOuterFrameActive);
        EditorGUILayout.LabelField("Outer Frame", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(outerScale, new GUIContent("Scale"));
        EditorGUILayout.PropertyField(outerFrameDepth, new GUIContent("Depth"));
        EditorGUI.indentLevel--;
        //EditorGUILayout.PropertyField(outerFrameMat, new GUIContent("Material"));
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!windowData.IsInnerFrameActive);
        EditorGUILayout.LabelField("Inner Frame", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(cols, new GUIContent("Columns"));
        EditorGUILayout.PropertyField(rows, new GUIContent("Rows"));
        EditorGUILayout.PropertyField(innerScale, new GUIContent("Scale"));
        EditorGUILayout.PropertyField(innerFrameDepth, new GUIContent("Depth"));
        EditorGUI.indentLevel--;
        //EditorGUILayout.PropertyField(innerFrameMat, new GUIContent("Material"));
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!windowData.IsPaneActive);
        EditorGUILayout.LabelField("Pane", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(paneDepth, new GUIContent("Depth"));
        EditorGUI.indentLevel--;
        //EditorGUILayout.PropertyField(paneMat, new GUIContent("Material"));
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!windowData.AreShuttersActive);
        EditorGUILayout.LabelField("Shutters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(shuttersDepth, new GUIContent("Depth"));
        EditorGUILayout.PropertyField(shuttersAngle, new GUIContent("Angle"));
        EditorGUI.indentLevel--;
        //EditorGUILayout.PropertyField(shuttersMat, new GUIContent("Material"));
        EditorGUI.EndDisabledGroup();

        if (serializedObject.ApplyModifiedProperties())
        {
            window.Build();
        }
    }
}
