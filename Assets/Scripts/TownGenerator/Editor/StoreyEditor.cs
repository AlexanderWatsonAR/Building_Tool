using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Storey))]
public class StoreyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Storey storey = (Storey)target;

        // Wall
        SerializedProperty wallDepth = serializedObject.FindProperty("m_WallDepth");
        SerializedProperty wallHeight = serializedObject.FindProperty("m_WallHeight");
        SerializedProperty wallMaterial = serializedObject.FindProperty("m_WallMaterial");

        // End Wall
        SerializedProperty floorHeight = serializedObject.FindProperty("m_FloorHeight");
        SerializedProperty floorMaterial = serializedObject.FindProperty("m_FloorMaterial");
        // Floor

        EditorGUILayout.LabelField("Wall");
        EditorGUILayout.PropertyField(wallDepth);
        EditorGUILayout.PropertyField(wallHeight);
        EditorGUILayout.PropertyField(wallMaterial);

        EditorGUILayout.LabelField("Floor");
        EditorGUILayout.PropertyField(floorHeight);
        EditorGUILayout.PropertyField(floorMaterial);

        float wDepth = storey.WallDepth;
        float wHeight = storey.WallHeight;

        float fHeight = storey.FloorHeight;

        if (serializedObject.ApplyModifiedProperties())
        {
            if (wallDepth.floatValue != wDepth ||
                wallHeight.floatValue != wHeight ||
                floorHeight.floatValue != fHeight)
            {
                storey.Build();
            }
        }

    }
}
