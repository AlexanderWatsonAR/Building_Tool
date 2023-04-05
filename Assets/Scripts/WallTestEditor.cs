using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WallTest))]
public class WallTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WallTest wallTest = (WallTest)target;

        SerializedProperty wallWidth = serializedObject.FindProperty("m_Width");
        SerializedProperty wallHeight = serializedObject.FindProperty("m_Height");
        SerializedProperty polyToolHole = serializedObject.FindProperty("m_HolePolytool");
        SerializedProperty material = serializedObject.FindProperty("m_Material");

        EditorGUILayout.PropertyField(wallWidth);
        EditorGUILayout.PropertyField(wallHeight);
        EditorGUILayout.PropertyField(polyToolHole);
        EditorGUILayout.PropertyField(material);

        if (GUILayout.Button("Do Wall Test"))
        {
            wallTest.CreateWallOutline();
        }

        float width = wallTest.Width;
        float height = wallTest.Height;

        if(serializedObject.ApplyModifiedProperties())
        {
            if (wallWidth.floatValue != width ||
                wallHeight.floatValue != height)
            {
                wallTest.CreateWallOutline();
            }
        }


    }

}
