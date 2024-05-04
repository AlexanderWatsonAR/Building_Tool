using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using OnlyInvalid.ProcGenBuilding.Building;
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
                building.Container = ScriptableObject.CreateInstance<BuildingScriptableObject>();
                building.Initialize(building.Container.Data);
                Selection.activeGameObject = building.gameObject;

                EditorApplication.delayCall += () =>
                {
                    ToolManager.SetActiveTool<DrawTool>();
                    DrawTool.OnStateChange.AddListener(BuildingEditor.DisplayMessages);
                    DrawTool.DrawState = DrawState.Draw;

                    building.Path.OnPointAdded.AddListener(() =>
                    {
                        if (building.Path.PathPointsCount == 1)
                            return;

                        // if the point is the same as the first point the polygon is complete.
                        if (Vector3.Distance(building.Path.GetFirstPosition(), building.Path.GetLastPosition()) <= building.Path.MinimumPointDistance)
                        {
                            building.Path.RemoveLastPoint();
                            DrawTool.DrawState = DrawState.Hide;
                            ToolManager.RestorePreviousPersistentTool();
                        }

                    });
                };
            }
        );

        newPolyBuilding_btn.text = "New Poly Building";

        Button save_btn = new Button
        (
            () =>
            {
                AssetDatabase.CreateAsset(m_ActiveBuilding.Container, "Assets/Export/" + m_ActiveBuilding.name + ".asset");
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
