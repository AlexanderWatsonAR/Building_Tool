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
    public class FrameDataDrawer : Polygon3DDataDrawer
    {
        FrameData m_PreviousData, m_CurrentData;

        FrameDataSerializedProperties m_Props;
        protected PropertyField m_Scale;

        protected override void Initialize(SerializedProperty data)
        {
            base.Initialize(data);
            m_Props = new FrameDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as FrameData;
            m_PreviousData = m_CurrentData.Clone() as FrameData;
        }
        protected override void DefineFields()
        {
            base.DefineFields();
            m_Scale = new PropertyField(m_Props.Scale);

            //m_Scale.SetEnabled(m_Buildable is not Frame);
        }
        protected override void BindFields()
        {
            base.BindFields();
            m_Scale.BindProperty(m_Props.Scale);
        }
        protected override void RegisterValueChangeCallbacks()
        {
            base.RegisterValueChangeCallbacks();

            m_Scale.RegisterValueChangeCallback(evt =>
            {
                float scale = evt.changedProperty.floatValue;

                if (scale == m_PreviousData.Scale)
                    return;

                m_PreviousData.Scale = scale;

                m_CurrentData.IsHoleDirty = true;
                m_CurrentData.IsDirty = true;
            });
        }
        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Scale);

            base.AddFieldsToRoot();
        }

    }
}
