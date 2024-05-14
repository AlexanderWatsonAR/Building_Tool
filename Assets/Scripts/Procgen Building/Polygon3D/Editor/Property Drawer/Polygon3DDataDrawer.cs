using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{

    [CustomPropertyDrawer(typeof(Polygon3DData), useForChildren:true)]
    public class Polygon3DDataDrawer : DataDrawer
    {
        Buildable m_Buildable;

        [SerializeField] Polygon3DData m_CurrentData;
        [SerializeField] Polygon3DData m_PreviousData;

        Polygon3DDataSerializedProperties m_Props;
        PropertyField m_Depth;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new Polygon3DDataSerializedProperties(data);
            m_Buildable = data.serializedObject.targetObject as Buildable;
            m_Root.name = nameof(FrameData) + "_Root";
            m_CurrentData = data.GetUnderlyingValue() as Polygon3DData;
            m_PreviousData = m_CurrentData.Clone() as Polygon3DData;
        }
        protected override void DefineFields()
        {
            m_Depth = new PropertyField(m_Props.Depth, "Depth");
        }
        protected override void BindFields()
        {
            m_Depth.BindProperty(m_Props.Depth);
        }
        protected override void RegisterValueChangeCallbacks()
        {
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
            m_Root.Add(m_Depth);
        }
    }
}
