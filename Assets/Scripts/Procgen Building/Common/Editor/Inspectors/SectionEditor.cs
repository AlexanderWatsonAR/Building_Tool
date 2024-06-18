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

        Button m_AddElementButton;

        public override VisualElement CreateInspectorGUI()
        {
            base.CreateInspectorGUI();

            CreateElementsButton();
            m_Root.Insert(0, m_AddElementButton);
            m_ContentMenuSystem.Initialize(m_AddElementButton);

            return m_Root;
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
