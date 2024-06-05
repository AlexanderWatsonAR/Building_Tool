using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using OnlyInvalid.CustomVisualElements;
using OnlyInvalid.ProcGenBuilding.Roof;
using System.Linq;
using System;
using OnlyInvalid.ProcGenBuilding.Building;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomEditor(typeof(Section))]
    public class SectionEditor : Editor
    {
        [SerializeField] VisualTreeAsset m_VisualTreeAsset;
        [SerializeField] StyleSheet m_StyleSheet;
        [SerializeField] Opening[] m_Openings;

        string[] m_Categories = new string[]
        {
            nameof(Opening),
        };


        VisualElement m_Root;
        VisualElement m_FloatingWindow;
        Button m_AddElementButton;

        SectionData m_SectionData;

        ListView m_CategoryList, m_ElementList;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();

            SerializedProperty buildables = serializedObject.FindProperty("m_Buildables");
            SerializedProperty sectionData = serializedObject.FindProperty("m_SectionData");
            SerializedProperty openings = sectionData.FindPropertyRelative("m_Openings");
            SerializedProperty firstOpening = openings.GetArrayElementAtIndex(0);

            m_SectionData = sectionData.GetUnderlyingValue() as SectionData;

            //m_Openings = Resources.FindObjectsOfTypeAll<Opening>();
            //object[] objects = Resources.LoadAll("Assets/Scripts/Procgen Building/Shape/SO/Resources", typeof(Opening));

            m_FloatingWindow = new VisualElement();
            m_FloatingWindow.Add(m_VisualTreeAsset.CloneTree());
            m_FloatingWindow.styleSheets.Add(m_StyleSheet);

            m_CategoryList = m_FloatingWindow.Q<ListView>("CategoryList");
            m_CategoryList.itemsSource = m_Categories;
            m_CategoryList.makeItem = () => new Label();
            m_CategoryList.bindItem = (element, i) => (element as Label).text = m_Categories[i];
            m_CategoryList.itemsChosen += CategoryList_onItemsChosen;


            m_ElementList = m_FloatingWindow.Q<ListView>("ElementList");
            m_ElementList.itemsChosen += ElementList_itemsChosen;
            //elementList.itemsSource = components;
            //elementList.makeItem = () => new Label();
            //elementList.bindItem = (element, i) => (element as Label).text = components[i].GetType().Name;

            // Think I may replace this method with a contextual one to simplify things.

            m_AddElementButton = new Button();
            m_AddElementButton.text = "Add Element";
            m_AddElementButton.clicked += ShowWindow;

            m_Root.Add(m_AddElementButton);


            for(int i = 0; i < openings.arraySize; i++)
            {
                SerializedProperty prop = openings.GetArrayElementAtIndex(i);

                HeaderFoldout foldout = new HeaderFoldout("Opening " + i.ToString());

                SerializedProperty height = prop.FindPropertyRelative("m_Height");
                PropertyField heightField = new PropertyField(height);

                foldout.Add(heightField);

                m_Root.Add(foldout);
            }


            //root.Add(foldout);

            return m_Root;
        }

        private void ElementList_itemsChosen(IEnumerable<object> obj)
        {
            string selectedElement = obj.ToArray()[0] as string;

            Opening aOpening = m_Openings[0].CloneViaFakeSerialization();
            
            m_SectionData.AddOpening(aOpening);
            m_Root.Remove(m_FloatingWindow);
        }

        private void CategoryList_onItemsChosen(IEnumerable<object> obj)
        {
            string selectedCategory = obj.ToArray()[0] as string;

            if (m_Categories.Any(x => x == selectedCategory))
            {
                m_ElementList.itemsSource = m_Openings;
                m_ElementList.Rebuild();
                m_CategoryList.style.display = DisplayStyle.None;
                m_ElementList.style.display = DisplayStyle.Flex;
            }
        }

        private void ShowWindow()
        {
            m_FloatingWindow.style.left = m_AddElementButton.worldBound.x;
            m_FloatingWindow.style.top = m_AddElementButton.worldBound.y + m_AddElementButton.resolvedStyle.height;

            m_Root.Add(m_FloatingWindow);
        }

    }
}
