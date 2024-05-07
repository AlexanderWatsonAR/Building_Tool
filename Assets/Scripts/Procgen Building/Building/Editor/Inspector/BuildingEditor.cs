using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Building
{
    [CustomEditor(typeof(Building))]
    public class BuildingEditor : Editor
    {
        Building m_Building;
        BuildingDataSerializedProperties m_Props;
        VisualElement m_Root;

        SerializedProperty m_Container, m_Data;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement() { name = nameof(Building) + "_Inspector"};

            DisplayMessages(DrawState.Hide);

            VisualElement horizontalWrapper = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1
                }
            };

            Button build_btn = new Button(() => m_Building.Build())
            {
                text = "Build"
            };
            Button reset_btn = new Button(() => m_Building.Demolish())
            {
                text = "Reset"
            };

            horizontalWrapper.Add(build_btn);
            horizontalWrapper.Add(reset_btn);

            m_Root.Add(horizontalWrapper);

            return m_Root;
        }
        private void ToolManager_activeToolChanged()
        {
            if(ToolManager.activeToolType == typeof(PolygonDrawTool))
            {
                DisplayMessages(DrawTool.DrawState);
                DrawTool.OnStateChange.AddListener(DisplayMessages);
                PolygonDrawTool.OnPolygonComplete.AddListener(Build);
            }
            else
            {
                m_Building.Path.OnPointAdded.RemoveListener(Build);
            }
        }
        public void Build()
        {
            m_Building.AddStorey("Ground");
            m_Building.InitializeRoof();
            m_Building.Build();
        }
        private void DisplayMessages(DrawState state)
        {
            m_Root.Clear();

            switch (state)
            {
                case DrawState.Draw:
                    {
                        m_Root.Add(new HelpBox("Click to draw points", HelpBoxMessageType.Info));
                    }
                    break;
                case DrawState.Edit:
                    {
                        BuildingData buildingData = m_Building.Data as BuildingData;
                        Button quit_btn = new Button(() =>
                        {
                            SceneView.RepaintAll();
                            ToolManager.RestorePreviousPersistentTool();
                            DisplayMessages(DrawState.Hide);
                        });
                        quit_btn.text = "Quit Edit";

                        Button remove_btn = new Button(() =>
                        {
                            buildingData.Path.RemovePointAt(DrawTool.SelectedHandle);

                            if (buildingData.Path.CheckPath())
                            {
                                m_Building.Build();
                            }

                            SceneView.RepaintAll();
                        });
                        remove_btn.text = "Remove Point";
                        remove_btn.SetEnabled(DrawTool.SelectedHandle < 0);

                        m_Root.Add(quit_btn);
                        m_Root.Add(remove_btn);
                        m_Root.Add(new HelpBox("Move points to update the building's shape", HelpBoxMessageType.Info));
                    }
                    break;
                case DrawState.Hide:
                    {
                        PropertyField container = new PropertyField(m_Container);
                        container.RegisterValueChangeCallback(evt =>
                        {
                            BuildingScriptableObject so = evt.changedProperty.GetUnderlyingValue() as BuildingScriptableObject;

                            if (so == null)
                                return;

                            m_Data.SetUnderlyingValue(so.Data);
                        });

                        Button edit_btn = new Button(() =>
                        {
                            ToolManager.SetActiveTool<DrawTool>();
                            DrawTool.OnStateChange.AddListener((drawState) => DisplayMessages(drawState));
                            SceneView.RepaintAll();
                        });
                        edit_btn.text = "Edit Building Path";

                        //m_Root.Add(containerField);
                        m_Root.Add(edit_btn);
                        m_Root.Add(new HelpBox("Editing the path will erase changes made to the building", HelpBoxMessageType.Warning));

                        ListView storeys = new ListView(m_Building.BuildingData.Storeys);

                        PropertyField roof = new PropertyField(m_Props.Roof.Data);
                        roof.BindProperty(m_Props.Roof.Data);

                        Foldout roofFoldout = new Foldout() { text = "Roof" };
                        roofFoldout.Add(roof);

                        m_Root.Add(container);
                        m_Root.Add(storeys);
                        m_Root.Add(roofFoldout);
                    }
                    break;
            }
        }
        private void OnEnable()
        {
            ToolManager.activeToolChanged += ToolManager_activeToolChanged;

            m_Building = target as Building;
            m_Container = serializedObject.FindProperty("m_DataAccessor");
            m_Data = serializedObject.FindProperty("m_BuildingData");
            m_Props = new BuildingDataSerializedProperties(m_Data);
        }
        private void OnDisable()
        {
            ToolManager.activeToolChanged -= ToolManager_activeToolChanged;
        }

    }
}
