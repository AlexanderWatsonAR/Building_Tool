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

        SerializedProperty WallWidth = serializedObject.FindProperty("m_Width");
        SerializedProperty WallHeight = serializedObject.FindProperty("m_Height");
        SerializedProperty Material = serializedObject.FindProperty("m_Material");

        EditorGUILayout.PropertyField(WallWidth);
        EditorGUILayout.PropertyField(WallHeight);
        EditorGUILayout.PropertyField(Material);

        if (GUILayout.Button("Do Wall Test"))
        {
            wallTest.CreateWallOutline();
        }

        float width = wallTest.Width;
        float height = wallTest.Height;

        if(serializedObject.ApplyModifiedProperties())
        {
            if (WallWidth.floatValue != width ||
                WallHeight.floatValue != height)
            {
                wallTest.CreateWallOutline();
            }
        }


    }

}
