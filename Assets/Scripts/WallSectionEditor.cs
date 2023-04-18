using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;

[CustomEditor(typeof(WallSection))]
public class WallSectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WallSection section = (WallSection)target;

        SerializedProperty columns = serializedObject.FindProperty("m_WindowColumns");
        SerializedProperty rows = serializedObject.FindProperty("m_WindowRows");
        SerializedProperty height = serializedObject.FindProperty("m_WindowHeight");
        SerializedProperty width = serializedObject.FindProperty("m_WindowWidth");
        SerializedProperty element = serializedObject.FindProperty("m_WallElement");

        EditorGUILayout.PropertyField(element);

        switch(section.WallElement)
        {
            case WallElement.Wall:
                break;
            case WallElement.Door:
                break;
            case WallElement.Window:
                EditorGUILayout.LabelField("Number of Windows");
                EditorGUILayout.IntSlider(columns, 1, 10, "Columns");
                EditorGUILayout.IntSlider(rows, 1, 10, "Rows");
                EditorGUILayout.LabelField("Size");
                EditorGUILayout.Slider(height, 0, 1, "Height");
                EditorGUILayout.Slider(width, 0, 1, "Width");
                break;
        }

        WallElement wallElement = section.WallElement;
        int wColumns = section.WindowColumns;
        int wRows = section.WindowRows;
        float wHeight = section.WindowHeight;
        float wWidth = section.WindowWidth;

        if (serializedObject.ApplyModifiedProperties())
        {
            if((WallElement)element.enumValueIndex != wallElement)
            {
                section.Build();
            }

            if(columns.intValue != wColumns ||
               rows.intValue != wRows ||
               height.floatValue != wHeight ||
               width.floatValue != wWidth)
            {
                section.Build();
            }
        }


    }
}
