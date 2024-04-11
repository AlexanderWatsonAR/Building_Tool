using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Roof
{
    [CustomPropertyDrawer(typeof(RoofSectionData))]
    public class RoofSectionDataDrawer : PropertyDrawer, IFieldInitializer
    {
        VisualElement m_Root;
        RoofSectionData m_PreviousData;
        PropertyField m_Columns, m_Rows;
        IBuildable m_Buildable;

        public override VisualElement CreatePropertyGUI(SerializedProperty data)
        {
            return base.CreatePropertyGUI(data);
        }

        public void Initialize(SerializedProperty data)
        {
            throw new System.NotImplementedException();
        }
        public void DefineFields()
        {
            throw new System.NotImplementedException();
        }
        public void BindFields()
        {
            throw new System.NotImplementedException();
        }
        public void AddFieldsToRoot()
        {
            throw new System.NotImplementedException();
        }
        public void RegisterValueChangeCallbacks()
        {
            throw new System.NotImplementedException();
        }
    }
}
