using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [CustomPropertyDrawer(typeof(FrameData), useForChildren:true)]
    public class FrameDataDrawer : DataDrawer
    {
        [SerializeField] FrameData m_PreviousData;
        [SerializeField] FrameData m_CurrentData;

        FrameDataSerializedProperties m_Props;
        PropertyField m_Depth, m_Scale;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new FrameDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as FrameData;
            m_PreviousData = m_CurrentData.Clone() as FrameData;
        }
        protected override void DefineFields()
        {
            m_Root.name = nameof(FrameData) + "_Root";
            m_Scale = new PropertyField(m_Props.Scale);
            m_Depth = new PropertyField(m_Props.Depth);

            //m_Scale.SetEnabled(m_Buildable is not Frame);
        }
        protected override void BindFields()
        {
            m_Scale.BindProperty(m_Props.Scale);
            m_Depth.BindProperty(m_Props.Depth);
        }
        protected override void RegisterValueChangeCallbacks()
        {
            m_Scale.RegisterValueChangeCallback(evt =>
            {
                float scale = evt.changedProperty.floatValue;

                if (scale == m_PreviousData.Scale)
                    return;

                m_PreviousData.Scale = scale;

                m_CurrentData.IsHoleDirty = true;
                m_CurrentData.IsDirty = true;
            });
            m_Depth.RegisterValueChangeCallback(evt =>
            {
                float depth = evt.changedProperty.floatValue;

                if (depth == m_PreviousData.Depth)
                    return;

                m_PreviousData.Depth = depth;

                m_CurrentData.IsDirty = true;
            });
        }
        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Scale);
            m_Root.Add(m_Depth);
        }

    }
}
