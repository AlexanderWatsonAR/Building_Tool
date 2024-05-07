using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using OnlyInvalid.ProcGenBuilding.Building;
using OnlyInvalid.ProcGenBuilding.Common;
using ToolManager = UnityEditor.EditorTools.ToolManager;

public class PolyBuildingEditorWindow : EditorWindow
{
    Building m_ActiveBuilding;
    bool m_IsActiveGameObjectABuilding;

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
                building.Initialize(new BuildingData());
                Selection.activeGameObject = building.gameObject;

                EditorApplication.delayCall += () =>
                {
                    ToolManager.SetActiveTool<PolygonDrawTool>();
                    DrawTool.DrawState = DrawState.Draw;
                };
            }
        );

        newPolyBuilding_btn.text = "New Poly Building";

        Button save_btn = new Button
        (
            () =>
            {
                AssetDatabase.CreateAsset(m_ActiveBuilding.DataAccessor, "Assets/Export/" + m_ActiveBuilding.name + ".asset");
            }
        ){ text = "Save Building"};


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
