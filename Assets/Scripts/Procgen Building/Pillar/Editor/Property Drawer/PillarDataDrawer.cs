using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [CustomPropertyDrawer(typeof(PillarData))]
    public class PillarDataDrawer : DataDrawer
    {
        PillarDataSerializedProperties m_Props;
        PillarData m_PreviousData;

        PropertyField m_Height, m_Width, m_Depth, m_Sides, m_IsSmooth;

        protected override void Initialize(SerializedProperty data)
        {
            m_Root.name = nameof(PillarData) + "_Root" ;
            m_Props = new PillarDataSerializedProperties(data);
            m_PreviousData = new PillarData(data.GetUnderlyingValue() as PillarData);
        }

        protected override void DefineFields()
        {
            m_Height = new PropertyField(m_Props.Height);
            m_Width = new PropertyField(m_Props.Width);
            m_Depth = new PropertyField(m_Props.Depth);
            m_Sides = new PropertyField(m_Props.Sides);
            m_IsSmooth = new PropertyField(m_Props.IsSmooth);
        }
        protected override void BindFields()
        {
            m_Height.BindProperty(m_Props.Height);
            m_Width.BindProperty(m_Props.Width);
            m_Depth.BindProperty(m_Props.Depth);
            m_Sides.BindProperty(m_Props.Sides);
            m_IsSmooth.BindProperty(m_Props.IsSmooth);
        }

        protected override void RegisterValueChangeCallbacks()
        {
            m_Height.RegisterValueChangeCallback(evt =>
            {
                float height = evt.changedProperty.floatValue;

                if (height == m_PreviousData.Height)
                    return;

                m_PreviousData.Height = height;
            });
            m_Depth.RegisterValueChangeCallback(evt =>
            {
                float depth = evt.changedProperty.floatValue;

                if (depth == m_PreviousData.Depth)
                    return;

                m_PreviousData.Depth = depth;
            });
            m_Width.RegisterValueChangeCallback(evt =>
            {
                float width = evt.changedProperty.floatValue;

                if (width == m_PreviousData.Width)
                    return;

                m_PreviousData.Width = width;
            });
            m_Sides.RegisterValueChangeCallback(evt =>
            {
                int sides = evt.changedProperty.intValue;

                if (sides == m_PreviousData.Depth)
                    return;

                m_PreviousData.Sides = sides;
            });
            m_IsSmooth.RegisterValueChangeCallback(evt =>
            {
                bool isSmooth = evt.changedProperty.boolValue;

                if (isSmooth == m_PreviousData.IsSmooth)
                    return;

                m_PreviousData.IsSmooth = isSmooth;
            });
        }

        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Height);
            m_Root.Add(m_Width);
            m_Root.Add(m_Depth);
            m_Root.Add(m_Sides);
            m_Root.Add(m_IsSmooth);
        }

    }
}
