using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(OpeningData), useForChildren: true)]
public abstract class OpeningDataDrawer : DataDrawer
{
    protected PropertyField m_Columns, m_Rows, m_Height, m_Width;

    OpeningDataSerializedProperties m_Props;
    OpeningData m_PreviousData;

    protected override void AddFieldsToRoot()
    {
    }

    protected override void BindFields()
    {
        m_Columns.BindProperty(m_Props.Columns);
        m_Rows.BindProperty(m_Props.Rows);
        m_Height.BindProperty(m_Props.Height);
        m_Width.BindProperty(m_Props.Width);
    }

    protected override void DefineFields()
    {
        m_Columns = new PropertyField();
        m_Rows = new PropertyField();
        m_Height = new PropertyField();
        m_Width = new PropertyField();
    }

    protected override void Initialize(SerializedProperty data)
    {
        m_Props = new OpeningDataSerializedProperties(data);
        OpeningData currentData = data.GetUnderlyingValue() as OpeningData;
        m_PreviousData = currentData.Clone() as OpeningData;
    }

    protected override void RegisterValueChangeCallbacks()
    {
        m_Columns.RegisterValueChangeCallback(evt =>
        {
            int columns = evt.changedProperty.intValue;

            if (columns == m_PreviousData.Columns)
                return;

            m_PreviousData.Columns = columns;
        });
        m_Rows.RegisterValueChangeCallback(evt =>
        {
            int rows = evt.changedProperty.intValue;

            if (rows == m_PreviousData.Rows)
                return;

            m_PreviousData.Rows = rows;
        });
        m_Height.RegisterValueChangeCallback(evt =>
        {
            float height = evt.changedProperty.floatValue;

            if (height == m_PreviousData.Height)
                return;

            m_PreviousData.Height = height;

        });
        m_Width.RegisterValueChangeCallback(evt =>
        {
            float width = evt.changedProperty.floatValue;

            if (width == m_PreviousData.Width)
                return;

            m_PreviousData.Width = width;
        });
    }
}