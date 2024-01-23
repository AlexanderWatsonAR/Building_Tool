using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;
using System.Linq;
using UnityEngine.Rendering;

public class MergeWindow : EditorWindow
{
    Building m_ActiveBuilding;
    private bool m_IsActiveGameObjectABuilding;

    // Are these storey parts included in the merge?
    private bool m_StoreysPrevious;
    private bool m_Storeys;
    private bool m_ExteriorWalls;
    private bool m_ExteriorWallCorners;
    private bool m_InteriorWalls;
    private bool m_Door;
    private bool m_DoorFrame;
    private bool m_DoorHandle;
    private bool m_Window;
    private bool m_OuterFrame;
    private bool m_InnerFrame;
    private bool m_Pane;
    private bool m_Shutters;
    private bool m_ShutterHandles;
    private bool m_Pillars;
    private bool m_Ceiling;
    private bool m_Floor;

    private bool m_Roof;

    public static void ShowWindow()
    {
        GetWindow(typeof(MergeWindow), false, "Merge");
    }

    private void OnGUI()
    {
        GUILayout.Label("Merge", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        GUILayout.Space(10);

        m_StoreysPrevious = m_Storeys;

        m_Storeys = EditorGUILayout.Toggle("Storey", m_Storeys);

        if (m_StoreysPrevious == false && m_Storeys == true)
        {
            m_ExteriorWalls = true;
            m_ExteriorWallCorners = true;
            m_InteriorWalls = true;
            m_InteriorWalls = true;
            m_Door = true;
            m_DoorFrame = true;
            m_DoorHandle = true;
            m_Window = true;
            m_OuterFrame = true;
            m_InnerFrame = true;
            m_Pane = true;
            m_Shutters = true;
            m_ShutterHandles = true;
            m_Pillars = true;
            m_Ceiling = true;
            m_Floor = true;
        }

        if (m_Storeys == false)
        {
            m_ExteriorWalls = false;
            m_ExteriorWallCorners = false;
            m_InteriorWalls = false;
            m_InteriorWalls = false;
            m_Door = false;
            m_DoorFrame = false;
            m_DoorHandle = false;
            m_Window = false;
            m_OuterFrame = false;
            m_InnerFrame = false;
            m_Pane = false;
            m_Shutters = false;
            m_ShutterHandles = false;
            m_Pillars = false;
            m_Ceiling = false;
            m_Floor = false;
        }
        

        EditorGUI.indentLevel++;
        m_ExteriorWalls = EditorGUILayout.Toggle("Exterior Walls", m_ExteriorWalls);
        m_ExteriorWallCorners = EditorGUILayout.Toggle("Exterior Wall Corners", m_ExteriorWallCorners);
        m_InteriorWalls = EditorGUILayout.Toggle("Interior Walls", m_InteriorWalls);
        m_Pillars = EditorGUILayout.Toggle("Pillars", m_Pillars);
        m_Ceiling = EditorGUILayout.Toggle("Ceiling", m_Ceiling);
        m_Floor = EditorGUILayout.Toggle("Floor", m_Floor);
        m_Door = EditorGUILayout.Toggle("Door", m_Door);
        EditorGUI.indentLevel++;
        m_DoorFrame = EditorGUILayout.Toggle("Frame", m_DoorFrame);
        m_DoorHandle = EditorGUILayout.Toggle("Handle", m_DoorHandle);
        EditorGUI.indentLevel--;
        m_Window = EditorGUILayout.Toggle("Window", m_Window);
        EditorGUI.indentLevel++;
        m_OuterFrame = EditorGUILayout.Toggle("Outer Frame", m_OuterFrame);
        m_InnerFrame = EditorGUILayout.Toggle("Inner Frame", m_InnerFrame);
        m_Pane = EditorGUILayout.Toggle("Pane", m_Pane);
        m_Shutters = EditorGUILayout.Toggle("Shutters", m_Shutters);
        m_ShutterHandles = EditorGUILayout.Toggle("Shutter Handles", m_ShutterHandles);
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        GUILayout.Space(10);
        m_Roof = EditorGUILayout.Toggle("Roof", m_Roof);
        GUILayout.Space(20);

        EditorGUI.indentLevel--;

        EditorGUI.BeginDisabledGroup(!m_IsActiveGameObjectABuilding);

        if (GUILayout.Button("Merge Building Components"))
        {
            Merge();
        }

        EditorGUI.EndDisabledGroup();
    }

    private void Merge()
    {
        ProBuilderMesh gameReadyBuilding = ProBuilderMesh.Create();
        gameReadyBuilding.name = m_ActiveBuilding.name + "_Merge";

        Pillar[] pillars = new Pillar[0];
        Window[] windows = new Window[0];
        WallSection[] wallSections = new WallSection[0];
        RoofSection[] roofSections = new RoofSection[0];

        if (m_Pillars)
        {
            pillars = m_ActiveBuilding.GetComponentsInChildren<Pillar>();
        }
        if(m_Window)
        {
            windows = m_ActiveBuilding.GetComponentsInChildren<Window>();
        }
        if(m_ExteriorWalls)
        {
            wallSections = m_ActiveBuilding.GetComponentsInChildren<WallSection>();
        }
        if(m_Roof)
        {
            roofSections = m_ActiveBuilding.GetComponentsInChildren<RoofSection>();
        }

        List<ProBuilderMesh> allBuildingBits = new();
        allBuildingBits.Add(gameReadyBuilding);

        foreach(Pillar pillar in pillars)
        {
            if(pillar.TryGetComponent(out ProBuilderMesh mesh))
            {
                if(mesh.positions.Count == 0)
                {
                    continue;
                }

                allBuildingBits.Add(mesh);
            }
        }

        foreach (Window window in windows)
        {
            if (window.TryGetComponent(out ProBuilderMesh mesh))
            {
                if (mesh.positions.Count == 0)
                {
                    continue;
                }

                allBuildingBits.Add(mesh);
            }
        }

        foreach (WallSection wall in wallSections)
        {
            if (wall.TryGetComponent(out ProBuilderMesh mesh))
            {
                if (mesh.positions.Count == 0)
                {
                    continue;
                }
                allBuildingBits.Add(mesh);
            }
        }

        foreach (RoofSection roofTile in roofSections)
        {
            if (roofTile.TryGetComponent(out ProBuilderMesh mesh))
            {
                if (mesh.positions.Count == 0)
                {
                    continue;
                }
                allBuildingBits.Add(mesh);
            }
        }

        CombineMeshes.Combine(allBuildingBits, gameReadyBuilding);
        gameReadyBuilding.ToMesh();
        gameReadyBuilding.Refresh();

        gameReadyBuilding.Optimize();
        gameReadyBuilding.ToMesh();
        gameReadyBuilding.Refresh();

        gameReadyBuilding.SetPivot(m_ActiveBuilding.transform.position);
    }

    private void OnSelectionChange()
    {
        m_IsActiveGameObjectABuilding = IsSelectedObjectABuilding();
    }

    private void OnEnable()
    {
        m_IsActiveGameObjectABuilding = IsSelectedObjectABuilding();
    }

    private bool IsSelectedObjectABuilding()
    {
        if (Selection.activeGameObject == null)
            return false;

        if (Selection.activeGameObject.TryGetComponent(out Building building))
        {
            m_ActiveBuilding = building;
            return true;
        }

        return false;
    }
}
