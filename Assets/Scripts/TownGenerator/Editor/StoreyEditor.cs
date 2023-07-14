using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;

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

        SerializedProperty activeElements = serializedObject.FindProperty("m_ActiveElements");

        // Wall
        SerializedProperty wallHeight = serializedObject.FindProperty("m_WallHeight");
        SerializedProperty wallDepth = serializedObject.FindProperty("m_WallDepth");
        SerializedProperty wallMaterial = serializedObject.FindProperty("m_WallMaterial");
        SerializedProperty curvedCorners = serializedObject.FindProperty("m_CurvedCorners");
        SerializedProperty curvedCornersSides = serializedObject.FindProperty("m_CurvedCornersSides");
        // End Wall

        // Floor
        SerializedProperty floorHeight = serializedObject.FindProperty("m_FloorHeight");
        SerializedProperty floorMaterial = serializedObject.FindProperty("m_FloorMaterial");
        // End Floor

        // Pillar
        SerializedProperty pillarWidth = serializedObject.FindProperty("m_PillarWidth");
        SerializedProperty pillarDepth = serializedObject.FindProperty("m_PillarDepth");
        SerializedProperty pillarMaterial = serializedObject.FindProperty("m_PillarMaterial");
        // End Pillar

        EditorGUILayout.PropertyField(activeElements);

        EditorGUI.BeginDisabledGroup(!storey.AreWallsActive);
        m_ShowWall = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowWall, "Wall");
        if (m_ShowWall)
        {
            EditorGUILayout.Slider(wallHeight, 1, 100, "Height");
            EditorGUILayout.Slider(wallDepth, 0.1f, 1, "Depth");
            EditorGUILayout.PropertyField(curvedCorners);
            EditorGUILayout.IntSlider(curvedCornersSides, 3, 15, "Sides");
            EditorGUILayout.ObjectField(wallMaterial, new GUIContent("Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!storey.IsFloorActive);
        m_ShowFloor = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowFloor, "Floor");
        if (m_ShowFloor)
        {
            EditorGUILayout.Slider(floorHeight, 0.00001f, 1, "Height");
            EditorGUILayout.ObjectField(floorMaterial, new GUIContent("Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!storey.ArePillarsActive);
        m_ShowPillar = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowPillar, "Pillar");
            if (m_ShowPillar)
            {
                EditorGUILayout.Slider(pillarWidth, 0, 10, "Width");
                EditorGUILayout.Slider(pillarDepth, 0, 10, "Depth");
                EditorGUILayout.ObjectField(pillarMaterial, new GUIContent("Material"));
            }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

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
