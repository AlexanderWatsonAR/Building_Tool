using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.PackageManager.UI;
using static UnityEngine.Rendering.DebugUI.Table;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [CustomPropertyDrawer(typeof(WindowOpeningData))]
    public class WindowOpeningDataDrawer : PropertyDrawer, IFieldInitializer
    {
        [SerializeField] WindowOpeningData m_CurrentData;
        [SerializeField] WindowOpeningData m_PreviousData;

        WindowOpeningDataSerializedProperties m_Props;

        VisualElement m_Root;
        Foldout m_GridFoldout, m_ShapeFoldout;
        PropertyField m_Columns, m_Rows, m_Sides, m_Height, m_Width, m_Angle, m_Window;

        public void AddFieldsToRoot()
        {
            m_GridFoldout.Add(m_Columns);
            m_GridFoldout.Add(m_Rows);
            m_ShapeFoldout.Add(m_Sides);
            m_ShapeFoldout.Add(m_Height);
            m_ShapeFoldout.Add(m_Width);
            m_ShapeFoldout.Add(m_Angle);
            m_Root.Add(m_GridFoldout);
            m_Root.Add(m_ShapeFoldout);
        }

        public void BindFields()
        {
            m_Columns.BindProperty(m_Props.Columns);
            m_Rows.BindProperty(m_Props.Rows);
            m_Sides.BindProperty(m_Props.Sides);
            m_Height.BindProperty(m_Props.Height);
            m_Width.BindProperty(m_Props.Width);
            m_Angle.BindProperty(m_Props.Angle);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty data)
        {
            Initialize(data);
            DefineFields();
            BindFields();
            RegisterValueChangeCallbacks();
            AddFieldsToRoot();
            return m_Root;

        }

        public void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(WindowOpeningData) + "_Root" };
            m_GridFoldout = new Foldout() { text = "Grid" };
            m_Columns = new PropertyField(m_Props.Columns) { label = "Columns" };
            m_Rows = new PropertyField(m_Props.Rows) { label = "Rows" };
            m_ShapeFoldout = new Foldout() { text = "Shape" };
            m_Sides = new PropertyField(m_Props.Sides) { label = "Sides" };
            m_Height = new PropertyField(m_Props.Height) { label = "Height" };
            m_Width = new PropertyField(m_Props.Width) { label = "Width" };
            m_Angle = new PropertyField(m_Props.Angle) { label = "Angle" };
        }

        public void Initialize(SerializedProperty data)
        {
            m_Props = new WindowOpeningDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as WindowOpeningData;
            m_PreviousData = m_CurrentData.Clone() as WindowOpeningData;
        }

        public void RegisterValueChangeCallbacks()
        {
            m_Columns.RegisterValueChangeCallback(evt =>
            {
                int columns = evt.changedProperty.intValue;

                if (columns == m_PreviousData.Columns)
                    return;

                m_PreviousData.Columns = columns;

                m_CurrentData.Windows = new WindowData[columns * m_CurrentData.Rows];
            });
            m_Rows.RegisterValueChangeCallback(evt =>
            {
                int rows = evt.changedProperty.intValue;

                if (rows == m_PreviousData.Rows)
                    return;

                m_PreviousData.Rows = rows;

                m_CurrentData.Windows = new WindowData[m_CurrentData.Columns * rows];
            });
            m_Sides.RegisterValueChangeCallback(evt =>
            {
                int sides = evt.changedProperty.intValue;
                if (sides == m_PreviousData.Sides)
                    return;

                m_PreviousData.Sides = sides;
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
            m_Angle.RegisterValueChangeCallback(evt =>
            {
                float angle = evt.changedProperty.floatValue;

                if (angle == m_PreviousData.Angle)
                    return;

                m_PreviousData.Angle = angle;
            });
        }
    }
}
