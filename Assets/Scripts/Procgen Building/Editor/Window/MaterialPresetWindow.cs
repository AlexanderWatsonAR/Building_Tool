using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using OnlyInvalid.ProcGenBuilding.Building;
using OnlyInvalid.ProcGenBuilding.Wall;

public class MaterialPresetWindow : EditorWindow
{
    private static Building m_ActiveBuilding;
    private static MaterialPalette m_CurrentPalette;

    private static string m_DefaultPaletteStr = "Assets/Default Material Palette.asset";

    [MenuItem("Tools/Material Preset Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(MaterialPresetWindow), false, "Material Preset");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Palette", EditorStyles.boldLabel);
        m_CurrentPalette = (MaterialPalette)EditorGUILayout.ObjectField(m_CurrentPalette, typeof(MaterialPalette), false);

        if (m_CurrentPalette == null)
        {
            m_CurrentPalette = AssetDatabase.LoadAssetAtPath<MaterialPalette>(m_DefaultPaletteStr);
        }

        if (m_CurrentPalette == null)
        {
            CreatePalette(m_DefaultPaletteStr);
        }

        if (m_CurrentPalette != null)
        {
            GUILayout.Label("Door", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            m_CurrentPalette.Door = (Material)EditorGUILayout.ObjectField("Door", m_CurrentPalette.Door, typeof(Material), false);
            m_CurrentPalette.DoorFrame = (Material)EditorGUILayout.ObjectField("Frame", m_CurrentPalette.DoorFrame, typeof(Material), false);
            m_CurrentPalette.DoorHandle = (Material)EditorGUILayout.ObjectField("Handle", m_CurrentPalette.DoorHandle, typeof(Material), false);
            EditorGUI.indentLevel--;
            GUILayout.Label("Window", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            m_CurrentPalette.OuterFrame = (Material)EditorGUILayout.ObjectField("Outer Frame", m_CurrentPalette.OuterFrame, typeof(Material), false);
            m_CurrentPalette.InnerFrame = (Material)EditorGUILayout.ObjectField("Inner Frame", m_CurrentPalette.InnerFrame, typeof(Material), false);
            m_CurrentPalette.Pane = (Material)EditorGUILayout.ObjectField("Pane", m_CurrentPalette.Pane, typeof(Material), false);
            m_CurrentPalette.Shutters = (Material)EditorGUILayout.ObjectField("Shutters", m_CurrentPalette.Shutters, typeof(Material), false);
            EditorGUI.indentLevel--;
            GUILayout.Label("Roof", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            m_CurrentPalette.ExteriorRoof = (Material)EditorGUILayout.ObjectField("Exterior", m_CurrentPalette.ExteriorRoof, typeof(Material), false);
            m_CurrentPalette.InteriorRoof = (Material)EditorGUILayout.ObjectField("Interior", m_CurrentPalette.InteriorRoof, typeof(Material), false);
            EditorGUI.indentLevel--;
            GUILayout.Label("Wall", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            m_CurrentPalette.ExteriorWall = (Material)EditorGUILayout.ObjectField("Exterior", m_CurrentPalette.OuterFrame, typeof(Material), false);
            m_CurrentPalette.ExteriorWallCorners = (Material)EditorGUILayout.ObjectField("Exterior Corners", m_CurrentPalette.InnerFrame, typeof(Material), false);
            m_CurrentPalette.InteriorWall = (Material)EditorGUILayout.ObjectField("Interior", m_CurrentPalette.Pane, typeof(Material), false);
            m_CurrentPalette.InteriorWallCorners = (Material)EditorGUILayout.ObjectField("Interior", m_CurrentPalette.Shutters, typeof(Material), false);
            EditorGUI.indentLevel--;
            GUILayout.Space(20);
            m_CurrentPalette.Pillar = (Material)EditorGUILayout.ObjectField("Pillar", m_CurrentPalette.OuterFrame, typeof(Material), false);
            m_CurrentPalette.Ceiling = (Material)EditorGUILayout.ObjectField("Ceiling", m_CurrentPalette.InnerFrame, typeof(Material), false);
            m_CurrentPalette.Floor = (Material)EditorGUILayout.ObjectField("Floor", m_CurrentPalette.Pane, typeof(Material), false);
        }

        if(GUILayout.Button("Create New Palette"))
        {
            CreatePalette(m_DefaultPaletteStr);
        }

        if (GUILayout.Button("Apply Palette to Building"))
        {
            if (m_CurrentPalette == null)
                return;

            if(Selection.activeGameObject.TryGetComponent(out Building building))
            {

                WallSection[] walls = building.gameObject.GetComponentsInChildren<WallSection>();
                foreach(WallSection wall in walls)
                {
                    wall.gameObject.GetComponent<Renderer>().sharedMaterial = m_CurrentPalette.ExteriorWall;
                }
            }
        }

    }

    private void CreatePalette(string path)
    {
        MaterialPalette asset = CreateInstance<MaterialPalette>();

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject.TryGetComponent(out Building building))
        {
            m_ActiveBuilding = building;
        }
    }

}
