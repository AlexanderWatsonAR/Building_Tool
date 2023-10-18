using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using Unity.VisualScripting;

public class PolyBuildingEditorWindow : EditorWindow
{
    Building m_ActiveBuilding;
    private bool m_IsActiveGameObjectABuilding;

    [MenuItem("Tools/PolyBuilding Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PolyBuildingEditorWindow), false, "PolyBuilding");
    }

    private void OnGUI()
    {
        if(GUILayout.Button("New Poly Building"))
        {
            ProBuilderMesh building = ProBuilderMesh.Create();
            building.name = "Poly Building";
            building.AddComponent<Building>();
            Building build = building.GetComponent<Building>();
            build.PolyPath.PolyMode = PolyMode.Draw;
            Selection.activeGameObject = building.gameObject;
        }

        EditorGUI.BeginDisabledGroup(!m_IsActiveGameObjectABuilding);

        if(GUILayout.Button("Merge"))
        {
            MergeWindow.ShowWindow();
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
