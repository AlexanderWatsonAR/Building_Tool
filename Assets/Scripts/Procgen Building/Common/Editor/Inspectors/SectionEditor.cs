using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using OnlyInvalid.CustomVisualElements;
using UnityEngine.ProBuilder;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomEditor(typeof(Section))]
    public class SectionEditor : BuildableEditor
    {
        [SerializeField] ListMenu m_ContentMenuSystem;

        Foldout m_SettingsFoldout;
        Button m_AddElementButton;

        public override VisualElement CreateInspectorGUI()
        {
            base.CreateInspectorGUI();


            DisplayConfig();
            CreateElementsButton();
            m_Root.Insert(0, m_SettingsFoldout);
            m_Root.Insert(0, m_AddElementButton);
            
            m_ContentMenuSystem.Initialize(m_AddElementButton);

            return m_Root;
        }

        private void DisplayConfig()
        {
            m_SettingsFoldout = new Foldout() { text = "Configure" };
            m_SettingsFoldout.style.marginBottom = 5;

            SerializedProperty isHorizontal = serializedObject.FindProperty("m_IsHorizontal");
            SerializedProperty isVertical = serializedObject.FindProperty("m_IsVertical");
            SerializedProperty isReversed = serializedObject.FindProperty("m_IsDirectionReversed");

            Label subdivide = new Label("Subdivide:");
            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();
            RadioButton horizontal = new RadioButton("Horizontally");
            RadioButton vertical = new RadioButton("Vertically");

            Toggle reversed = new Toggle("Reverse");

            horizontal.BindProperty(isHorizontal);
            vertical.BindProperty(isVertical);
            reversed.BindProperty(isReversed);

            radioButtonGroup.Add(horizontal);
            radioButtonGroup.Add(vertical);

            horizontal.RegisterValueChangedCallback(evt =>
            {
                isVertical.SetUnderlyingValue(!evt.newValue);
            });
            vertical.RegisterValueChangedCallback(evt =>
            {
                isHorizontal.SetUnderlyingValue(!evt.newValue);
            });

            m_SettingsFoldout.Add(subdivide);
            m_SettingsFoldout.Add(radioButtonGroup);
            m_SettingsFoldout.Add(reversed);
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
            m_AddElementButton.clicked += () =>
            {
                m_ContentMenuSystem.CreateMenu();
                m_ContentMenuSystem.Menu.DropDown(m_AddElementButton.worldBound, m_AddElementButton);
            };
        }
    }


}
