using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Door door = (Door)target;

        SerializedProperty doorHeight = serializedObject.FindProperty("m_HeightScale");
        SerializedProperty doorWidth = serializedObject.FindProperty("m_WidthScale");
        SerializedProperty doorDepth = serializedObject.FindProperty("m_DepthScale");

        EditorGUILayout.Slider(doorHeight, 0, 1, "Height");
        EditorGUILayout.Slider(doorWidth, 0, 1, "Width");
        EditorGUILayout.Slider(doorDepth, 0, 1, "Depth");

        if (serializedObject.ApplyModifiedProperties())
        {
            door.Build();
        }



    }
}
