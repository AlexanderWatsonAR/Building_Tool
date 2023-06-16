using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Storey))]
public class StoreyEditor : Editor
{
    private bool m_ShowWall = true;
    private bool m_ShowFloor = true;
    private bool m_ShowPillar = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Storey storey = (Storey)target;

        // Wall
        SerializedProperty wallHeight = serializedObject.FindProperty("m_WallHeight");
        SerializedProperty wallDepth = serializedObject.FindProperty("m_WallDepth");
        SerializedProperty wallMaterial = serializedObject.FindProperty("m_WallMaterial");
        // End Wall

        // Floor
        SerializedProperty floorHeight = serializedObject.FindProperty("m_FloorHeight");
        SerializedProperty floorMaterial = serializedObject.FindProperty("m_FloorMaterial");
        // End Floor

        // Pillar
        SerializedProperty pillarsActive = serializedObject.FindProperty("m_ArePillarsActive");
        SerializedProperty pillarWidth = serializedObject.FindProperty("m_PillarWidth");
        SerializedProperty pillarDepth = serializedObject.FindProperty("m_PillarDepth");
        SerializedProperty pillarMaterial = serializedObject.FindProperty("m_PillarMaterial");
        // End Pillar

        m_ShowWall = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowWall, "Wall");
        if (m_ShowWall)
        {
            EditorGUILayout.Slider(wallHeight, 1, 100, "Height");
            EditorGUILayout.Slider(wallDepth, 0, 1, "Depth");
            EditorGUILayout.ObjectField(wallMaterial, new GUIContent("Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        m_ShowFloor = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowFloor, "Floor");
        if (m_ShowFloor)
        {
            EditorGUILayout.Slider(floorHeight, 0.00001f, 1, "Height");
            EditorGUILayout.ObjectField(floorMaterial, new GUIContent("Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        pillarsActive.boolValue = EditorGUILayout.Toggle("Are Pillars Active", pillarsActive.boolValue);

        m_ShowPillar = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowPillar, "Pillar");
        if (pillarsActive.boolValue)
        {
            if (m_ShowPillar)
            {
                EditorGUILayout.Slider(pillarWidth, 0, 10, "Width");
                EditorGUILayout.Slider(pillarDepth, 0, 10, "Depth");
                EditorGUILayout.ObjectField(pillarMaterial, new GUIContent("Material"));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        if (serializedObject.ApplyModifiedProperties())
        {
            if (storey.TryGetComponent(out Building building))
            {
                building.Build();
            }
            else
            {
                storey.Build();
            }
        }

    }
}
