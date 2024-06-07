using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomPropertyDrawer(typeof(OpeningData), useForChildren: true)]
    public class OpeningDataDrawer : DataDrawer
    {
        protected PropertyField /*m_Columns, m_Rows,*/ m_Shape, m_Height, m_Width, m_Angle, m_Position;

        OpeningDataSerializedProperties m_Props;
        //OpeningData m_PreviousData;

        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Shape);
            m_Root.Add(m_Height);
            m_Root.Add(m_Width);
            m_Root.Add(m_Angle);
            m_Root.Add(m_Position);
        }

        protected override void BindFields()
        {
            //m_Columns.BindProperty(m_Props.Columns);
            //m_Rows.BindProperty(m_Props.Rows);
            m_Shape.BindProperty(m_Props.Shape);
            m_Height.BindProperty(m_Props.Height);
            m_Width.BindProperty(m_Props.Width);
            m_Angle.BindProperty(m_Props.Angle);
            m_Position.BindProperty(m_Props.Position);
        }

        protected override void DefineFields()
        {
            //m_Columns = new PropertyField();
            //m_Rows = new PropertyField();
            m_Shape = new PropertyField();
            m_Height = new PropertyField();
            m_Width = new PropertyField();
            m_Angle = new PropertyField();
            m_Position = new PropertyField();
        }

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new OpeningDataSerializedProperties(data);
            //OpeningData currentData = data.GetUnderlyingValue() as OpeningData;
            //m_PreviousData = currentData.Clone() as OpeningData;
        }

        protected override void RegisterValueChangeCallbacks()
        {
            //m_Columns.RegisterValueChangeCallback(evt =>
            //{
            //    int columns = evt.changedProperty.intValue;

            //    if (columns == m_PreviousData.Columns)
            //        return;

            //    m_PreviousData.Columns = columns;
            //});
            //m_Rows.RegisterValueChangeCallback(evt =>
            //{
            //    int rows = evt.changedProperty.intValue;

            //    if (rows == m_PreviousData.Rows)
            //        return;

            //    m_PreviousData.Rows = rows;
            //});
            //m_Height.RegisterValueChangeCallback(evt =>
            //{
            //    float height = evt.changedProperty.floatValue;

            //    if (height == m_PreviousData.Height)
            //        return;

            //    m_PreviousData.Height = height;

            //});
            //m_Width.RegisterValueChangeCallback(evt =>
            //{
            //    float width = evt.changedProperty.floatValue;

            //    if (width == m_PreviousData.Width)
            //        return;

            //    m_PreviousData.Width = width;
            //});
        }
    }
}