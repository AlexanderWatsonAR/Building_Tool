using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    [CustomPropertyDrawer(typeof(CornersData))]
    public class CornersDataDrawer : PropertyDrawer, IFieldInitializer
    {
        VisualElement m_Root;
        CornersDataSerializedProperties m_Props;
        PropertyField m_Corner;

        public override VisualElement CreatePropertyGUI(SerializedProperty data)
        {
            Initialize(data);
            DefineFields();
            BindFields();
            RegisterValueChangeCallbacks();
            AddFieldsToRoot();

            return m_Root;
        }
        public void Initialize(SerializedProperty data)
        {
            m_Props = new CornersDataSerializedProperties(data);
        }
        public void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(CornersData) + "_Root" };
            m_Corner = new PropertyField(m_Props.Corner.Data);
        }
        public void BindFields()
        {
            m_Corner.BindProperty(m_Props.Corner.Data);
        }
        public void RegisterValueChangeCallbacks()
        {
            throw new System.NotImplementedException();
        }
        public void AddFieldsToRoot()
        {
            m_Corner.Add(m_Corner);
        }
    }
}
