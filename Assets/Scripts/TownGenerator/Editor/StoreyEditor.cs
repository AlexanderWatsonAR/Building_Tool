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

        if (storey.TryGetComponent(out Building building))
        {
            if(!building.HasInitialized)
                return;

            BuildingSerializedProperties props = new(building);

            props.SerializedObject.Update();

            EditorGUILayout.PropertyField(props.Storeys);

            if (props.SerializedObject.ApplyModifiedProperties())
            {
                for(int i = 0; i < building.Data.StoreysData.Count; i++)
                {
                    building.Data.StoreysData[i].ID = i;
                }

                building.Build();
            }
        }
        else
        {
            DisplayStorey(storey);
        }

    }

    private void DisplayStorey(Storey storey)
    {
        SerializedProperty data = serializedObject.FindProperty("m_Data");

        SerializedProperty activeElements = data.FindPropertyRelative("m_ActiveElements");

        #region Wall
        SerializedProperty wallData = data.FindPropertyRelative("m_Wall");
        SerializedProperty wallHeight = wallData.FindPropertyRelative("m_Height");
        SerializedProperty wallDepth = wallData.FindPropertyRelative("m_Depth");
        SerializedProperty wallMaterial = wallData.FindPropertyRelative("m_Material");
        #endregion

        #region Corner
        SerializedProperty cornerData = data.FindPropertyRelative("m_Corner");
        SerializedProperty cornerType = cornerData.FindPropertyRelative("m_Type");
        SerializedProperty cornerSides = cornerData.FindPropertyRelative("m_Sides");
        #endregion

        #region Floor
        SerializedProperty floorData = data.FindPropertyRelative("m_Floor");
        SerializedProperty floorHeight = floorData.FindPropertyRelative("m_Height");
        SerializedProperty floorMaterial = floorData.FindPropertyRelative("m_Material");
        #endregion

        #region Pillar
        SerializedProperty pillarData = data.FindPropertyRelative("m_Pillar");
        SerializedProperty pillarWidth = pillarData.FindPropertyRelative("m_Width");
        SerializedProperty pillarDepth = pillarData.FindPropertyRelative("m_Depth");
        SerializedProperty pillarSides = pillarData.FindPropertyRelative("m_Sides");
        SerializedProperty pillarSmooth = pillarData.FindPropertyRelative("m_IsSmooth");
        SerializedProperty pillarMaterial = pillarData.FindPropertyRelative("m_Material");
        #endregion

        EditorGUILayout.PropertyField(activeElements);

        StoreyElement elements = activeElements.GetEnumValue<StoreyElement>();

        EditorGUI.BeginDisabledGroup(!elements.IsElementActive(StoreyElement.Walls));
        m_ShowWall = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowWall, "Wall");
        if (m_ShowWall)
        {
            if (!storey.TryGetComponent(out WallSection _))
            {
                EditorGUILayout.Slider(wallHeight, 1, 100, "Height");
            }
            EditorGUILayout.Slider(wallDepth, 0.1f, 1, "Depth");
            EditorGUILayout.PropertyField(cornerType);
            EditorGUILayout.IntSlider(cornerSides, 3, 15, "Sides");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!elements.IsElementActive(StoreyElement.Floor));
        m_ShowFloor = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowFloor, "Floor");
        if (m_ShowFloor)
        {
            EditorGUILayout.Slider(floorHeight, 0.00001f, 1, "Height");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!elements.IsElementActive(StoreyElement.Pillars));
        m_ShowPillar = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowPillar, "Pillar");
        if (m_ShowPillar)
        {
            EditorGUILayout.Slider(pillarWidth, 0, 10, "Width");
            EditorGUILayout.Slider(pillarDepth, 0, 10, "Depth");
            EditorGUILayout.IntSlider(pillarSides, 3, 32, "Sides");
            pillarSmooth.boolValue = EditorGUILayout.Toggle("Is Smooth", pillarSmooth.boolValue);

        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.EndDisabledGroup();

        if (serializedObject.ApplyModifiedProperties())
        {
            if (storey.TryGetComponent(out Building building))
            {
                building.Build();
            }
            else if (storey.TryGetComponent(out WallSection wallSection)) // If the storey is an extension. 
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
