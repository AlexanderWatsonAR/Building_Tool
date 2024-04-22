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
    public class FrameDataDrawer : PropertyDrawer, IFieldInitializer
    {
        IBuildable m_Buildable;
        [SerializeField] FrameData m_PreviousData;
        [SerializeField] FrameData m_CurrentData;

        FrameDataSerializedProperties m_Props;
        VisualElement m_Root;
        PropertyField m_Depth, m_Scale;

        public override VisualElement CreatePropertyGUI(SerializedProperty data)
        {
            Initialize(data);
            DefineFields();
            BindFields();
            RegisterValueChangeCallbacks();
            AddFieldsToRoot();

            return m_Root;
        }

        public void Initialize(SerializedProperty data)
        {
            m_Props = new FrameDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as FrameData;
            m_PreviousData = m_CurrentData.Clone() as FrameData;
            m_Buildable = data.serializedObject.targetObject as IBuildable;
        }

        public void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(FrameData) + "_Root" };
            m_Scale = new PropertyField(m_Props.Scale);
            m_Depth = new PropertyField(m_Props.Depth);

            m_Scale.SetEnabled(m_Buildable is not Frame);
        }

        public void BindFields()
        {
            m_Scale.BindProperty(m_Props.Scale);
            m_Depth.BindProperty(m_Props.Depth);
        }

        public void RegisterValueChangeCallbacks()
        {
            m_Scale.RegisterValueChangeCallback(evt =>
            {
                float scale = evt.changedProperty.floatValue;

                if (scale == m_PreviousData.Scale)
                    return;

                m_PreviousData.Scale = scale;

                m_CurrentData.Holes = new PolygonData[1];

                m_CurrentData.Holes[0] = new PolygonData(m_CurrentData.Polygon.ControlPoints.ScalePolygon(m_CurrentData.Scale, m_CurrentData.Position), m_CurrentData.Polygon.Normal);

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

        public void AddFieldsToRoot()
        {
            m_Root.Add(m_Scale);
            m_Root.Add(m_Depth);
        }

    }
}
