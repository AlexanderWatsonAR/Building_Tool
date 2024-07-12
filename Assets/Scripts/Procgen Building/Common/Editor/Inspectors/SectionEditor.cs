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

            Transform t = (target as Buildable).transform;
            string viewDataKey = "SettingsFoldout";

            while (t != null)
            {
                viewDataKey += t.name;
                t = t.parent;
            }

            m_SettingsFoldout.viewDataKey = viewDataKey;
            m_SettingsFoldout.style.marginBottom = 5;

            SerializedProperty isHorizontal = serializedObject.FindProperty("m_IsHorizontal");
            SerializedProperty isVertical = serializedObject.FindProperty("m_IsVertical");
            SerializedProperty isReversed = serializedObject.FindProperty("m_IsDirectionReversed");

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();
            RadioButton horizontal = new RadioButton("Horizontally");
            RadioButton vertical = new RadioButton("Vertically");

            Toggle reversed = new Toggle("Reverse");

            horizontal.BindProperty(isHorizontal);
            vertical.BindProperty(isVertical);
            reversed.BindProperty(isReversed);

            radioButtonGroup.Add(horizontal);
            radioButtonGroup.Add(vertical);

            SectionData data = m_Data.GetUnderlyingValue() as SectionData;

            horizontal.RegisterValueChangedCallback(evt =>
            {
                isVertical.SetUnderlyingValue(!evt.newValue);
                data.IsDirty = true;
            });
            vertical.RegisterValueChangedCallback(evt =>
            {
                isHorizontal.SetUnderlyingValue(!evt.newValue);
                data.IsDirty = true;
            });
            reversed.RegisterValueChangedCallback(evt => 
            {
                data.IsDirty = true;
            });

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
