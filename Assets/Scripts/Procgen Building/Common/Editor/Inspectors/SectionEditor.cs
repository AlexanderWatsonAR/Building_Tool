using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using OnlyInvalid.CustomVisualElements;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomEditor(typeof(Section))]
    public class SectionEditor : Editor
    {
        [SerializeField] OpeningSO[] m_SavedOpenings;
        [SerializeField] SerializedProperty m_SectionData;
        [SerializeField] SerializedProperty m_SectionOpenings;

        VisualElement m_Root, m_Stack;
        Button m_AddElementButton;

        GenericDropdownMenu m_ElementMenu, m_ShapeMenu, m_PolygonMenu;

        OpeningData m_TestOpening;
        NPolygon m_TestNPolygon;

        public override VisualElement CreateInspectorGUI()
        {
            Initalize();
            BuildDisplay();

            Button buildButton = new Button(() =>
            {
                Section section = (Section)target;
                section.SectionData.IsDirty = true;
                section.Build();

            });
            buildButton.text = "Build";

            m_Root.Add(buildButton);

            return m_Root;
        }

        private void Initalize()
        {
            m_Root = new VisualElement();
            m_Stack = new VisualElement();
            m_SectionData = serializedObject.FindProperty("m_Data");
            m_SectionOpenings = m_SectionData.FindPropertyRelative("m_Openings");
            m_SavedOpenings = Resources.FindObjectsOfTypeAll<OpeningSO>();
            CreateElementsMenu();
            CreateShapeMenu();
            CreatePolygonMenu();
            //object[] objects = Resources.LoadAll("Assets/Scripts/Procgen Building/Shape/SO/Resources", typeof(Opening));
        }

        private void BuildDisplay()
        {
            m_Root.Clear();
            CreateElementsButton();
            CreateStack();
        }

        private void CreateElementsButton()
        {
            m_AddElementButton = new Button()
            {
                style =
                {
                    marginTop = 5,
                    marginBottom = 10
                }
            };
            m_AddElementButton.text = "Add Element";
            m_AddElementButton.clicked += () => { m_ElementMenu.DropDown(m_AddElementButton.worldBound, m_AddElementButton); };

            m_Root.Add(m_AddElementButton);
        }

        private void CreateStack()
        {
            SectionData sectionData = m_SectionData.GetUnderlyingValue() as SectionData;

            for (int i = 0; i < m_SectionOpenings.arraySize; i++)
            {
                SerializedProperty opening = m_SectionOpenings.GetArrayElementAtIndex(i);

                HeaderFoldout foldout = new HeaderFoldout("Opening " + i.ToString());

                VerticalContainer content = new VerticalContainer();

                SerializedProperty shape = opening.FindPropertyRelative("m_Shape");
                SerializedProperty height = opening.FindPropertyRelative("m_Height");
                SerializedProperty width = opening.FindPropertyRelative("m_Width");
                SerializedProperty angle = opening.FindPropertyRelative("m_Angle");
                SerializedProperty position = opening.FindPropertyRelative("m_Position");

                PropertyField shapeField = new PropertyField(shape) { label = shape.boxedValue.ToString() };
                PropertyField heightField = new PropertyField(height);
                PropertyField widthField = new PropertyField(width);
                PropertyField angleField = new PropertyField(angle);
                PropertyField positionField = new PropertyField(position);

                content.Add(shapeField);
                content.Add(heightField);
                content.Add(widthField);
                content.Add(angleField);
                content.Add(positionField);
                foldout.AddItem(content);

                foldout.contextMenu.AddItem("Remove Item", false, () => { sectionData.RemoveOpening(opening.GetUnderlyingValue() as OpeningData); });

                m_Stack.Add(foldout);
            }

            m_Root.Add(m_Stack);
        }

        private void CreateElementsMenu()
        {
            m_ElementMenu = new GenericDropdownMenu();
            m_ElementMenu.contentContainer.Add(new Label("Elements") { style = { alignSelf = Align.Center, unityFontStyleAndWeight = FontStyle.Bold } });
            m_ElementMenu.AddSeparator("");
            m_ElementMenu.AddItem("Opening", false, () => m_ShapeMenu.DropDown(m_AddElementButton.worldBound, m_AddElementButton));
        }

        private void CreateShapeMenu()
        {
            m_ShapeMenu = new GenericDropdownMenu();
            m_ShapeMenu.contentContainer.Add(new Label("Shape") { style = { alignSelf = Align.Center, unityFontStyleAndWeight = FontStyle.Bold } });
            m_ShapeMenu.AddSeparator("");
            m_ShapeMenu.AddItem("Polygon", false, () => m_PolygonMenu.DropDown(m_AddElementButton.worldBound, m_AddElementButton));
        }

        private void CreatePolygonMenu()
        {
            SectionData sectionData = m_SectionData.GetUnderlyingValue() as SectionData;
            m_PolygonMenu = new GenericDropdownMenu();
            m_PolygonMenu.contentContainer.Add(new Label("Polygon") { style = { alignSelf = Align.Center, unityFontStyleAndWeight = FontStyle.Bold } });
            m_PolygonMenu.AddSeparator("");
            m_PolygonMenu.AddItem("Triangle", false, () => { sectionData.AddOpening(new OpeningData(new NPolygon(3))); });
            m_PolygonMenu.AddItem("Square", false, () => sectionData.AddOpening(new OpeningData(new NPolygon(4))));
            m_PolygonMenu.AddItem("Pentagon", false, () => sectionData.AddOpening(new OpeningData(new NPolygon(5))));
            m_PolygonMenu.AddItem("Hexagon", false, () => sectionData.AddOpening(new OpeningData(new NPolygon(6))));
            m_PolygonMenu.AddItem("Septagon", false, () => sectionData.AddOpening(new OpeningData(new NPolygon(7))));
            m_PolygonMenu.AddItem("Octagon", false, () => sectionData.AddOpening(new OpeningData(new NPolygon(8))));
        }
    }


}
