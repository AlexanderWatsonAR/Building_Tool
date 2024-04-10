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

    [CustomPropertyDrawer(typeof(RoofTileData))]
    public class RoofTileDataDrawer : PropertyDrawer, IFieldInitializer
    {
        VisualElement m_Root;
        RoofTileData m_PreviousData;
        RoofTileDataSerializedProperties m_Props;
        PropertyField m_Columns, m_Rows;
        IBuildable m_Buildable;

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
            m_Props = new RoofTileDataSerializedProperties(data);
            m_Buildable = data.serializedObject.targetObject as IBuildable;
            m_PreviousData = data.GetUnderlyingValue() as RoofTileData;

        }
        public void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(RoofTileData) + "_Root" };
            m_Columns = new PropertyField(m_Props.Columns);
            m_Rows = new PropertyField(m_Props.Rows);
        }
        public void BindFields()
        {
            m_Columns.BindProperty(m_Props.Columns);
            m_Rows.BindProperty(m_Props.Rows);
        }
        public void RegisterValueChangeCallbacks()
        {
            m_Columns.RegisterValueChangeCallback(evt =>
            {
                int columns = evt.changedProperty.intValue;

                if (columns == m_PreviousData.Columns)
                    return;

                m_Buildable.Build();
            });

            m_Rows.RegisterValueChangeCallback(evt =>
            {
                int rows = evt.changedProperty.intValue;

                if (rows == m_PreviousData.Rows)
                    return;

                m_Buildable.Build();
            });
        }
        public void AddFieldsToRoot()
        {
            m_Root.Add(m_Columns);
            m_Root.Add(m_Rows);
        }
    }
}
