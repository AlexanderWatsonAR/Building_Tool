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
    public class RoofTileDataDrawer : DataDrawer
    {
        RoofTileData m_PreviousData;
        RoofTileDataSerializedProperties m_Props;
        PropertyField m_Columns, m_Rows;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new RoofTileDataSerializedProperties(data);
            m_PreviousData = data.GetUnderlyingValue() as RoofTileData;

        }
        protected override void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(RoofTileData) + "_Root" };
            m_Columns = new PropertyField(m_Props.Columns);
            m_Rows = new PropertyField(m_Props.Rows);
        }
        protected override void BindFields()
        {
            m_Columns.BindProperty(m_Props.Columns);
            m_Rows.BindProperty(m_Props.Rows);
        }
        protected override void RegisterValueChangeCallbacks()
        {
            m_Columns.RegisterValueChangeCallback(evt =>
            {
                int columns = evt.changedProperty.intValue;

                if (columns == m_PreviousData.Columns)
                    return;
            });

            m_Rows.RegisterValueChangeCallback(evt =>
            {
                int rows = evt.changedProperty.intValue;

                if (rows == m_PreviousData.Rows)
                    return;
            });
        }
        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Columns);
            m_Root.Add(m_Rows);
        }
    }
}
