using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

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
            //m_MeshesToCombine.Clear();
            ProBuilderMesh gameReadyBuilding = ProBuilderMesh.Create();
            gameReadyBuilding.name = m_ActiveBuilding.name + " GR";

            List<ProBuilderMesh> allBuildingBits = m_ActiveBuilding.gameObject.GetComponentsInChildren<ProBuilderMesh>().ToList();
            List<ProBuilderMesh> usableBuildingBits = new List<ProBuilderMesh>();

            usableBuildingBits.Add(gameReadyBuilding);

            foreach (ProBuilderMesh mesh in allBuildingBits)
            {
                if(mesh.positions.Count > 0)
                {
                    usableBuildingBits.Add(mesh);
                }
            }

            //FindProBuilderMeshesInHierarchy(m_ActiveBuilding.transform);

            List<ProBuilderMesh> output = CombineMeshes.Combine(usableBuildingBits, gameReadyBuilding);
            gameReadyBuilding.ToMesh();
            gameReadyBuilding.Refresh();
            
        }

        if (GUILayout.Button("Export"))
        {
            ExportEditorWindow.ShowWindow();
        }

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Material Presets"))
        {
            MaterialPresetWindow.ShowWindow();

        }
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
        if (Selection.activeGameObject == null)
            return;

        if(Selection.activeGameObject.TryGetComponent(out Building building))
            m_IsActiveGameObjectABuilding = true;

        if(m_IsActiveGameObjectABuilding)
            m_ActiveBuilding = building;
    }

}
