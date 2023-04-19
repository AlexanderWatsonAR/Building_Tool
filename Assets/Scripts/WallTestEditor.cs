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

        SerializedProperty wallDepth = serializedObject.FindProperty("m_Depth");
        SerializedProperty wallHeight = serializedObject.FindProperty("m_Height");
        SerializedProperty holeHeight = serializedObject.FindProperty("m_HoleHeight");
        SerializedProperty holeWidth = serializedObject.FindProperty("m_HoleWidth");
        SerializedProperty holeRotation = serializedObject.FindProperty("m_HoleRotation");
        SerializedProperty holeColumns = serializedObject.FindProperty("m_HoleColumns");
        SerializedProperty holeRows = serializedObject.FindProperty("m_HoleRows");
        SerializedProperty material = serializedObject.FindProperty("m_Material");

        EditorGUILayout.LabelField("Wall");
        EditorGUILayout.PropertyField(wallDepth);
        EditorGUILayout.PropertyField(wallHeight);
        EditorGUILayout.LabelField("Hole");
        EditorGUILayout.PropertyField(holeHeight);
        EditorGUILayout.PropertyField(holeWidth);
        EditorGUILayout.PropertyField(holeRotation);
        EditorGUILayout.PropertyField(holeColumns);
        EditorGUILayout.PropertyField(holeRows);
        EditorGUILayout.PropertyField(material);

        if (GUILayout.Button("Do Wall Test"))
        {
            wallTest.CreateWallOutline();
        }

        float depth = wallTest.Depth;
        float height = wallTest.Height;
        float hHeight = wallTest.HoleHeight;
        float hWidth = wallTest.HoleWidth;
        float hRotation = wallTest.HoleRotation;
        int hRows = wallTest.HoleRows;
        int hColumns = wallTest.HoleColumns;

        if (serializedObject.ApplyModifiedProperties())
        {
            if (wallDepth.floatValue != depth ||
                wallHeight.floatValue != height ||
                holeHeight.floatValue != hHeight ||
                holeWidth.floatValue != hWidth ||
                holeRotation.floatValue != hRotation ||
                holeColumns.intValue != hColumns ||
                holeRows.intValue != hRows)
            {
                wallTest.CreateWallOutline();
            }
        }


    }

}
