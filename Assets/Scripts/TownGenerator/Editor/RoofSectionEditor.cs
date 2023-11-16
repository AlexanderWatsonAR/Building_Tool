using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;

[CustomEditor(typeof(RoofSection))]
public class RoofSectionEditor : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        RoofSection section = (RoofSection)target;

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        SerializedProperty element = data.FindPropertyRelative("m_RoofElement");

        SerializedProperty winColumns = data.FindPropertyRelative("m_WindowColumns");
        SerializedProperty winRows = data.FindPropertyRelative("m_WindowRows");
        SerializedProperty winHeight = data.FindPropertyRelative("m_WindowHeight");
        SerializedProperty winWidth = data.FindPropertyRelative("m_WindowWidth");
        SerializedProperty winSides = data.FindPropertyRelative("m_WindowSides");

        EditorGUILayout.PropertyField(element);

        switch (element.GetEnumValue<RoofElement>())
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
