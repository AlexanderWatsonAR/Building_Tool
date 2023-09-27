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
        SerializedProperty wallData = serializedObject.FindProperty("m_WallData");
        SerializedProperty wallHeight = wallData.FindPropertyRelative("m_Height");
        SerializedProperty wallDepth = wallData.FindPropertyRelative("m_Depth");
        SerializedProperty wallMaterial = wallData.FindPropertyRelative("m_Material");
        SerializedProperty curvedCorners = serializedObject.FindProperty("m_CurvedCorners");
        SerializedProperty curvedCornersSides = serializedObject.FindProperty("m_CurvedCornersSides");
        // End Wall

        // Floor
        SerializedProperty floorHeight = serializedObject.FindProperty("m_FloorHeight");
        SerializedProperty floorMaterial = serializedObject.FindProperty("m_FloorMaterial");
        // End Floor

        // Pillar
        SerializedProperty pillarData = serializedObject.FindProperty("m_PillarData");

        SerializedProperty pillarWidth = pillarData.FindPropertyRelative("m_Width");
        SerializedProperty pillarDepth = pillarData.FindPropertyRelative("m_Depth");
        SerializedProperty pillarSides = pillarData.FindPropertyRelative("m_Sides");
        SerializedProperty pillarSmooth = pillarData.FindPropertyRelative("m_IsSmooth");
        SerializedProperty pillarMaterial = pillarData.FindPropertyRelative("m_Material");
        // End Pillar

        EditorGUILayout.PropertyField(activeElements);

        EditorGUI.BeginDisabledGroup(!storey.AreWallsActive);
        m_ShowWall = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowWall, "Wall");
        if (m_ShowWall)
        {
            if(!storey.TryGetComponent(out WallSection _))
            {
                EditorGUILayout.Slider(wallHeight, 1, 100, "Height");
            }
            EditorGUILayout.Slider(wallDepth, 0.1f, 1, "Depth");
            EditorGUILayout.PropertyField(curvedCorners);
            EditorGUILayout.IntSlider(curvedCornersSides, 3, 15, "Sides");
            //EditorGUILayout.ObjectField(wallMaterial, new GUIContent("Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!storey.IsFloorActive);
        m_ShowFloor = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowFloor, "Floor");
        if (m_ShowFloor)
        {
            EditorGUILayout.Slider(floorHeight, 0.00001f, 1, "Height");
            //EditorGUILayout.ObjectField(floorMaterial, new GUIContent("Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!storey.ArePillarsActive);
        m_ShowPillar = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowPillar, "Pillar");
        if (m_ShowPillar)
        {
            //bool enterChildren = true;    
            //while(pillarData.Next(enterChildren))
            //{
            //    enterChildren = false;
            //    EditorGUILayout.PropertyField(pillarData);
            //}
            EditorGUILayout.Slider(pillarWidth, 0, 10, "Width");
            EditorGUILayout.Slider(pillarDepth, 0, 10, "Depth");
            EditorGUILayout.IntSlider(pillarSides, 3, 32, "Sides");
            pillarSmooth.boolValue = EditorGUILayout.Toggle("Is Smooth", pillarSmooth.boolValue);
            //EditorGUILayout.ObjectField(pillarMaterial, new GUIContent("Material"));

        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        if (serializedObject.ApplyModifiedProperties())
        {
            if (storey.TryGetComponent(out Building building))
            {
                building.Build();
            }
            else if(storey.TryGetComponent(out WallSection wallSection)) // If the storey is an extension. 
            {
                wallSection.Build();
            }
            else
            {
                storey.Build();
            }
        }

    }
}
