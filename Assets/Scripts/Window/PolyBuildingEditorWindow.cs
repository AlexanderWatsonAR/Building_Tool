using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class PolyBuildingEditorWindow : EditorWindow
{
    Building m_ActiveBuilding;
    private bool m_IsActiveGameObjectABuilding;
    List<ProBuilderMesh> m_MeshesToCombine = new();

    [MenuItem("Tools/PolyBuilding Window")]

    public static void ShowWindow()
    {
        GetWindow(typeof(PolyBuildingEditorWindow), false, "PolyBuilding");
    }

    private void OnGUI()
    {
        if(GUILayout.Button("New Poly Building"))
        {
            GameObject building = new GameObject("Poly Building", typeof(Building));
            Building build = building.GetComponent<Building>();
            build.PolyPath.PolyMode = PolyMode.Draw;
            Selection.activeGameObject = building;
        }

        EditorGUI.BeginDisabledGroup(!m_IsActiveGameObjectABuilding);

        if(GUILayout.Button("Make Game Ready"))
        {
            m_MeshesToCombine.Clear();
            ProBuilderMesh gameReadyBuilding = ProBuilderMesh.Create();

            FindProBuilderMeshesInHierarchy(m_ActiveBuilding.transform);

            CombineMeshes.Combine(m_MeshesToCombine, gameReadyBuilding);
            gameReadyBuilding.ToMesh();
            gameReadyBuilding.Refresh();
            
        }

        if (GUILayout.Button("Export"))
        {
            ExportEditorWindow.ShowWindow();
        }

        EditorGUI.EndDisabledGroup();

        //GUILayout.Label("Heading", EditorStyles.boldLabel);
        //GUILayout.Label(text);
        //toggle = EditorGUILayout.BeginToggleGroup("Settings", toggle);

        //customText = EditorGUILayout.TextField("Text Field", customText);
        //slider = EditorGUILayout.IntSlider("Custom Slider", slider, -5, 5);

        //EditorGUILayout.EndToggleGroup();


    }

    private void FindProBuilderMeshesInHierarchy(Transform parent)
    {
        if (parent.TryGetComponent(out ProBuilderMesh mesh))
        {
            m_MeshesToCombine.Add(mesh);
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // Recursively traverse the child's hierarchy
            FindProBuilderMeshesInHierarchy(child);
        }
    }

    private void OnSelectionChange()
    {
        m_IsActiveGameObjectABuilding = Selection.activeGameObject.TryGetComponent(out Building building);

        if(m_IsActiveGameObjectABuilding)
            m_ActiveBuilding = building;
    }

}
