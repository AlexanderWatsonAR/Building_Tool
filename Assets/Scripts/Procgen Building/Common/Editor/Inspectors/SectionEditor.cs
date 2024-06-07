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
    public class SectionEditor : BuildableEditor
    {
     //   [SerializeField] OpeningSO[] m_SavedOpenings;

        Button m_AddElementButton;

        GenericDropdownMenu m_ElementMenu, m_ShapeMenu, m_PolygonMenu;


        public override VisualElement CreateInspectorGUI()
        {
            base.CreateInspectorGUI();
            
            Initalize();
            m_Root.Insert(0, m_AddElementButton);

            return m_Root;
        }

        private void Initalize()
        {
           // m_SavedOpenings = Resources.FindObjectsOfTypeAll<OpeningSO>();
            CreateElementsButton();
            CreateElementsMenu();
            CreateShapeMenu();
            CreatePolygonMenu();
            //object[] objects = Resources.LoadAll("Assets/Scripts/Procgen Building/Shape/SO/Resources", typeof(Opening));
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
            SectionData sectionData = m_Data.GetUnderlyingValue() as SectionData;
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
