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
                BuildingData data = building.Data as BuildingData;
                data.Path.PolyMode = PolyMode.Draw;
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
        ){ text = "Save Building"};


        Button newDrawable = new Button
        (
            () =>
            {
                GameObject drawable = new GameObject("Drawable", typeof(Drawable));
                Selection.activeGameObject = drawable;

                EditorApplication.delayCall += () =>
                {
                    DrawTool.Activate(DrawState.Draw);
                    Drawable draw = drawable.GetComponent<Drawable>();
                    draw.Path.OnPointAdded.AddListener(() =>
                    {
                        if (draw.Path.PathPointsCount == 1)
                            return;

                        // TODO: if the point is the same as the first point the polygon is complete.
                        if (Vector3.Distance(draw.Path.GetFirstPosition(), draw.Path.GetLastPosition()) <= draw.Path.MinimumPointDistance)
                        {
                            draw.Path.RemoveLastPoint();
                            DrawTool.DrawState = DrawState.Hide;
                            ToolManager.RestorePreviousPersistentTool();
                        }

                    });
                };
                
            }
        ){ text = "New Drawable"};

        rootVisualElement.Add(newPolyBuilding_btn);
        rootVisualElement.Add(save_btn);
        rootVisualElement.Add(newDrawable);
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
