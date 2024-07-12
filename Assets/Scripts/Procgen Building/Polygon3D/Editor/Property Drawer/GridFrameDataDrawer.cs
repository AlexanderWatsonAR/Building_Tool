using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [CustomPropertyDrawer(typeof(GridFrameData))]
    public class GridFrameDataDrawer : FrameDataDrawer
    {
        [SerializeField] GridFrameData m_CurrentData;
        [SerializeField] GridFrameData m_PreviousData;

        GridFrameDataSerializedProperties m_Props;
        Foldout m_Grid;
        PropertyField m_Columns, m_Rows;

        protected override void Initialize(SerializedProperty data)
        {
            base.Initialize(data);
            m_Props = new GridFrameDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as GridFrameData;
            m_PreviousData = m_CurrentData.Clone() as GridFrameData;
        }
        protected override void DefineFields()
        {
            base.DefineFields();
            m_Grid = new Foldout() { text = "Grid" };
            m_Columns = new PropertyField(m_Props.Columns, "Columns");
            m_Rows = new PropertyField(m_Props.Rows, "Rows");
        }
        protected override void BindFields()
        {
            base.BindFields();
            m_Columns.BindProperty(m_Props.Columns);
            m_Rows.BindProperty(m_Props.Rows);
        }

        protected override void RegisterValueChangeCallbacks()
        {
            base.RegisterValueChangeCallbacks();

            m_Columns.RegisterValueChangeCallback(evt =>
            {
                if (evt.changedProperty.intValue == m_PreviousData.Columns)
                    return;

                m_PreviousData.Columns = evt.changedProperty.intValue;

                m_CurrentData.IsHoleDirty = true;
                m_CurrentData.IsDirty = true;
            });
            m_Rows.RegisterValueChangeCallback(evt =>
            {
                if (evt.changedProperty.intValue == m_PreviousData.Rows)
                    return;

                m_PreviousData.Rows = evt.changedProperty.intValue;

                m_CurrentData.IsHoleDirty = true;
                m_CurrentData.IsDirty = true;
            });
        }
        protected override void AddFieldsToRoot()
        {
            m_Grid.Add(m_Columns);
            m_Grid.Add(m_Rows);
            m_Root.Add(m_Grid);

            base.AddFieldsToRoot();
        }

    }
}
