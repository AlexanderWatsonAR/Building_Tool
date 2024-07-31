using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using OnlyInvalid.CustomVisualElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomPropertyDrawer(typeof(SectionData), false)]
    public class SectionDataDrawer : DataDrawer
    {
        SectionDataSerializedProperties m_Props;
        PropertyField m_Openings;

        protected override void AddFieldsToRoot()
        {
            m_Root.Clear();
            m_Root.Add(m_Openings);
        }

        protected override void BindFields()
        {
            m_Openings.BindProperty(m_Props.Openings);
        }

        protected override void DefineFields()
        {
            m_Openings = new PropertyField();
        }

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new SectionDataSerializedProperties(data);
        }

        protected override void RegisterValueChangeCallbacks()
        {
        }
    }
}