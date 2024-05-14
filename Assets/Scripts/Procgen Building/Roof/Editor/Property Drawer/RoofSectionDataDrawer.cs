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
    public class RoofSectionDataDrawer : DataDrawer
    {
        RoofSectionData m_PreviousData;
        PropertyField m_Columns, m_Rows;

        protected override void Initialize(SerializedProperty data)
        {
            throw new System.NotImplementedException();
        }
        protected override void DefineFields()
        {
            throw new System.NotImplementedException();
        }
        protected override void BindFields()
        {
            throw new System.NotImplementedException();
        }
        protected override void AddFieldsToRoot()
        {
            throw new System.NotImplementedException();
        }
        protected override void RegisterValueChangeCallbacks()
        {
            throw new System.NotImplementedException();
        }
    }
}
