using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    [CustomPropertyDrawer(typeof(CornersData))]
    public class CornersDataDrawer : DataDrawer
    {
        CornersDataSerializedProperties m_Props;
        PropertyField m_Corner;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new CornersDataSerializedProperties(data);
        }
        protected override void DefineFields()
        {
            m_Corner = new PropertyField(m_Props.Corner.Data);
        }
        protected override void BindFields()
        {
            m_Corner.BindProperty(m_Props.Corner.Data);
        }
        protected override void RegisterValueChangeCallbacks()
        {
            throw new System.NotImplementedException();
        }
        protected override void AddFieldsToRoot()
        {
            m_Corner.Add(m_Corner);
        }
    }
}
