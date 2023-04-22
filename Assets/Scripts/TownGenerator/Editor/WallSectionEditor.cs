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

        SerializedProperty element = serializedObject.FindProperty("m_WallElement");

        // Door
        SerializedProperty doorColumns = serializedObject.FindProperty("m_DoorColumns");
        SerializedProperty doorRows = serializedObject.FindProperty("m_DoorRows");
        SerializedProperty doorHeight = serializedObject.FindProperty("m_PedimentHeight");
        SerializedProperty doorWidth = serializedObject.FindProperty("m_SideWidth");
        // End Door

        // Window
        SerializedProperty winColumns = serializedObject.FindProperty("m_WindowColumns");
        SerializedProperty winRows = serializedObject.FindProperty("m_WindowRows");
        SerializedProperty winHeight = serializedObject.FindProperty("m_WindowHeight");
        SerializedProperty winWidth = serializedObject.FindProperty("m_WindowWidth");
        // End Window

        EditorGUILayout.PropertyField(element);

        switch(section.WallElement)
        {
            case WallElement.Wall:
                break;
            case WallElement.Door:
                EditorGUILayout.LabelField("Number of Doors");
                EditorGUILayout.IntSlider(doorColumns, 1, 10, "Columns");
                EditorGUILayout.IntSlider(doorRows, 1, 10, "Rows");
                EditorGUILayout.LabelField("Size");
                EditorGUILayout.PropertyField(doorHeight);
                EditorGUILayout.PropertyField(doorWidth);
                break;
            case WallElement.Window:
                EditorGUILayout.LabelField("Number of Windows");
                EditorGUILayout.IntSlider(winColumns, 1, 10, "Columns");
                EditorGUILayout.IntSlider(winRows, 1, 10, "Rows");
                EditorGUILayout.LabelField("Size");
                EditorGUILayout.Slider(winHeight, 0, 1, "Height");
                EditorGUILayout.Slider(winWidth, 0, 1, "Width");
                break;
        }

        WallElement wallElement = section.WallElement;
        // Window
        int wColumns = section.WindowColumns;
        int wRows = section.WindowRows;
        float wHeight = section.WindowHeight;
        float wWidth = section.WindowWidth;
        // End Window

        // Door
        int dColumns = section.DoorColumns;
        int dRows = section.DoorRows;
        float dHeight = section.PedimentHeight;
        float dWidth = section.SideWidth;


        if (serializedObject.ApplyModifiedProperties())
        {
            if((WallElement)element.enumValueIndex != wallElement)
            {
                section.Build();
            }

            if(winColumns.intValue != wColumns ||
               winRows.intValue != wRows ||
               winHeight.floatValue != wHeight ||
               winWidth.floatValue != wWidth)
            {
                section.Build();
            }

            if (doorColumns.intValue != dColumns ||
               doorRows.intValue != dRows ||
               doorHeight.floatValue != dHeight ||
               doorWidth.floatValue != dWidth)
            {
                section.Build();
            }
        }


    }
}
