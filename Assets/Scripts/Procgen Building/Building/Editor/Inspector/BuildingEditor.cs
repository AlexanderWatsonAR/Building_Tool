using UnityEngine;
using UnityEditor;

using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Building
{
    [CustomEditor(typeof(Building))]
    public class BuildingEditor : Editor
    {
        Building m_Building;
        BuildingDataSerializedProperties m_Props;

        VisualElement m_Root;
        [SerializeField] int m_SelectedHandle = -1;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();

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

        private void DisplayMessages(PolyMode polyMode)
        {
            m_Root.Clear();

            switch (polyMode)
            {
                case PolyMode.Draw:
                    {
                        m_Root.Add(new HelpBox("Click to draw points", HelpBoxMessageType.Info));
                    }
                    break;
                case PolyMode.Edit:
                    {
                        BuildingData buildingData = m_Building.Data as BuildingData;
                        Button quit_btn = new Button(() =>
                        {
                            SceneView.RepaintAll();
                            DisplayMessages(buildingData.Path.PolyMode);
                        });
                        quit_btn.text = "Quit Edit";

                        Button remove_btn = new Button(() =>
                        {
                            buildingData.Path.RemoveControlPointAt(m_SelectedHandle - 1);

                            if (buildingData.Path.IsValidPath())
                            {
                                m_Building.Build();
                            }

                            SceneView.RepaintAll();
                        });
                        remove_btn.text = "Remove Point";
                        remove_btn.SetEnabled(m_SelectedHandle == -1);

                        m_Root.Add(quit_btn);
                        m_Root.Add(remove_btn);
                        m_Root.Add(new HelpBox("Move points to update the building's shape", HelpBoxMessageType.Info));
                    }
                    break;
                case PolyMode.Hide:
                    {
                        //SerializedProperty container = serializedObject.FindProperty("m_Container");
                        //PropertyField containerField = new PropertyField(container);
                        //containerField.RegisterValueChangeCallback(evt =>
                        //{
                        //    BuildingScriptableObject so = evt.changedProperty.GetUnderlyingValue() as BuildingScriptableObject;

                        //    if (so == null)
                        //        return;

                        //    m_Data.SetUnderlyingValue(so.Data);
                        //    Debug.Log("building data changed");

                        //});

                        Button edit_btn = new Button(() =>
                        {
                            //m_Path.PolyMode = PolyMode.Edit;
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

                        m_Root.Add(storeys);
                        m_Root.Add(roofFoldout);
                    }
                    break;
            }
        }

        private void OnEnable()
        {
            m_Building = target as Building;

            SerializedProperty container = serializedObject.FindProperty("m_DataContainer");
            SerializedProperty data = container.FindPropertyRelative("m_BuildingData");
            m_Props = new BuildingDataSerializedProperties(data);
            m_Building.BuildingData.Path.OnPolyModeChanged.AddListener(DisplayMessages);
        }
        private void OnDisable()
        {
            m_Building.BuildingData.Path.OnPolyModeChanged.RemoveListener(DisplayMessages);
        }

    }
}
