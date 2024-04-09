using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class PolyBuildingEditorWindow : EditorWindow
{
    Building m_ActiveBuilding;
    private bool m_IsActiveGameObjectABuilding;

    [MenuItem("Tools/PolyBuilding Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PolyBuildingEditorWindow), false, "PolyBuilding");
    }

    public void CreateGUI()
    {
        Button newPolyBuilding_btn = new Button
        (
            () =>
            {
                ProBuilderMesh buildingMesh = ProBuilderMesh.Create();
                buildingMesh.name = "Poly Building";
                buildingMesh.AddComponent<Building>();
                Building building = buildingMesh.GetComponent<Building>();
                building.Container = ScriptableObject.CreateInstance<BuildingScriptableObject>();
                building.Initialize(building.Container.Data);
                building.Data.Path.PolyMode = PolyMode.Draw;
                Selection.activeGameObject = building.gameObject;
            }
        );

       newPolyBuilding_btn.text = "New Poly Building";

        Button save_btn = new Button
        (
            () =>
            {
                AssetDatabase.CreateAsset(m_ActiveBuilding.Container, "Assets/Export/" + m_ActiveBuilding.name + ".asset");
            }
        )
        { text = "Save Building"};
       

       rootVisualElement.Add(newPolyBuilding_btn);
       rootVisualElement.Add(save_btn);
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
