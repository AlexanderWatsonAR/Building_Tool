using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;

[CustomEditor(typeof(WallSection))]
public class WallSectionEditor : Editor
{
    private bool m_IsDoorActiveFoldout = true;

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
        SerializedProperty doorActive = serializedObject.FindProperty("m_IsDoorActive");
        SerializedProperty doorScale = serializedObject.FindProperty("m_DoorScale");
        SerializedProperty doorMaterial = serializedObject.FindProperty("m_DoorMaterial");
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

                doorActive.boolValue = EditorGUILayout.Toggle("Is Active", doorActive.boolValue);

                if (doorActive.boolValue)
                {
                    m_IsDoorActiveFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDoorActiveFoldout, "Door");

                    if (m_IsDoorActiveFoldout)
                    {
                        doorScale.vector3Value = EditorGUILayout.Vector3Field("Scale", doorScale.vector3Value);
                        //EditorGUILayout.BeginHorizontal();
                        //float x = EditorGUILayout.Slider("x", doorScale.vector3Value.x, 0, 1);
                        //float y = EditorGUILayout.Slider("y", doorScale.vector3Value.y, 0, 1);
                        //float z = EditorGUILayout.Slider("z", doorScale.vector3Value.z, 0, 1);
                        //doorScale.vector3Value = new Vector3(x, y, z);
                        //EditorGUILayout.EndHorizontal();

                        EditorGUILayout.ObjectField(doorMaterial, new GUIContent("Material"));
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
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


        if (serializedObject.ApplyModifiedProperties())
        {
            section.Build();
        }


    }
}
