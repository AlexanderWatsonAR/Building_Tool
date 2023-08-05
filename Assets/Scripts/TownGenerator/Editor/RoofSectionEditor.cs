using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoofSection))]
public class RoofSectionEditor : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        RoofSection section = (RoofSection)target;

        SerializedProperty element = serializedObject.FindProperty("m_RoofElement");

        SerializedProperty winColumns = serializedObject.FindProperty("m_WindowColumns");
        SerializedProperty winRows = serializedObject.FindProperty("m_WindowRows");
        SerializedProperty winHeight = serializedObject.FindProperty("m_WindowHeight");
        SerializedProperty winWidth = serializedObject.FindProperty("m_WindowWidth");
        SerializedProperty winSides = serializedObject.FindProperty("m_WindowSides");

        EditorGUILayout.PropertyField(element);

        switch (section.RoofElement)
        {
            case RoofElement.Tile:
                break;
            case RoofElement.Window:
                EditorGUILayout.LabelField("Number of Windows");
                EditorGUILayout.IntSlider(winColumns, 1, 10, "Columns");
                EditorGUILayout.IntSlider(winRows, 1, 10, "Rows");
                EditorGUILayout.IntSlider(winSides, 3, 32, "Sides");
                EditorGUILayout.LabelField("Size");
                EditorGUILayout.Slider(winHeight, 0, 0.999f, "Height");
                EditorGUILayout.Slider(winWidth, 0, 0.999f, "Width");
                break;
            case RoofElement.Empty:
                break;
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            section.Build();
        }
    }
}
