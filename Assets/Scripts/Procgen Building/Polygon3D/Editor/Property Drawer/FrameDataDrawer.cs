using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [CustomPropertyDrawer(typeof(FrameData))]
    public class FrameDataDrawer : DataDrawer
    {
        FrameDataSerializedProperties m_Props;
        Slider m_Depth, m_Scale;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new FrameDataSerializedProperties(data);
        }
        protected override void DefineFields()
        {
            var displayData = DisplayDataSettings.Data.Frame;

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
            m_Scale.BindProperty(m_Props.Scale);
            m_Depth.BindProperty(m_Props.Depth);
        }
        protected override void RegisterValueChangeCallbacks()
        {
        }
        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Scale);
            m_Root.Add(m_Depth);
        }

    }
}
