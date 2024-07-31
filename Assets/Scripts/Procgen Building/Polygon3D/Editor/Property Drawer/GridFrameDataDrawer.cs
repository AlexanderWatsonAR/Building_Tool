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
    public class GridFrameDataDrawer : DataDrawer
    {
        GridFrameDataSerializedProperties m_Props;
        Foldout m_Grid;
        Slider m_Depth, m_Scale;
        SliderInt m_Columns, m_Rows;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new GridFrameDataSerializedProperties(data);
        }
        protected override void DefineFields()
        {
            var displayData = DisplayDataSettings.Data.GridFrame;

            m_Grid = new Foldout()
            {
                text = "Grid"
            };
            m_Columns = new SliderInt()
            {
                label = displayData.Columns.label,
                lowValue = displayData.Columns.range.lower,
                highValue = displayData.Columns.range.upper,
                direction = displayData.Columns.direction,
                inverted = displayData.Columns.inverted,
                showInputField = displayData.Columns.showInputField,
            };
            m_Rows = new SliderInt()
            {
                label = displayData.Rows.label,
                lowValue = displayData.Rows.range.lower,
                highValue = displayData.Rows.range.upper,
                direction = displayData.Rows.direction,
                inverted = displayData.Rows.inverted,
                showInputField = displayData.Rows.showInputField,
            };
            m_Scale = new Slider()
            {
                label = displayData.Scale.label,
                lowValue = displayData.Scale.range.lower,
                highValue = displayData.Scale.range.upper,
                direction = displayData.Scale.direction,
                showInputField = displayData.Scale.showInputField,
                inverted = displayData.Scale.inverted
            };
            m_Depth = new Slider()
            {
                label = displayData.Depth.label,
                lowValue = displayData.Depth.range.lower,
                highValue = displayData.Depth.range.upper,
                direction = displayData.Depth.direction,
                showInputField = displayData.Depth.showInputField,
                inverted = displayData.Depth.inverted
            };
        }
        protected override void BindFields()
        {
            m_Columns.BindProperty(m_Props.Columns);
            m_Rows.BindProperty(m_Props.Rows);
            m_Depth.BindProperty(m_Props.Depth);
            m_Scale.BindProperty(m_Props.Scale);
        }

        protected override void RegisterValueChangeCallbacks()
        {
        }
        protected override void AddFieldsToRoot()
        {
            m_Grid.Add(m_Columns);
            m_Grid.Add(m_Rows);
            m_Root.Add(m_Grid);
            m_Root.Add(m_Scale);
            m_Root.Add(m_Depth);
        }

    }
}
