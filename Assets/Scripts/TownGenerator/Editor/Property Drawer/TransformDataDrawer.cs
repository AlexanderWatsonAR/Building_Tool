using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEditor.Rendering;

[CustomPropertyDrawer(typeof(TransformData))]
public class TransformDataDrawer : PropertyDrawer, IFieldInitializer
{
    [SerializeField] TransformData m_PreviousData;
    TransformDataSerializedProperties m_Props;

    VisualElement m_Root;
    PropertyField m_RelativePosition, m_Offset, m_EulerAngle;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();
        return m_Root;
    }

    public void AddFieldsToRoot()
    {
        m_Root.Add(m_RelativePosition);
        m_Root.Add(m_Offset);
        m_Root.Add(m_EulerAngle);
    }

    public void BindFields()
    {
        m_RelativePosition.BindProperty(m_Props.RelativePosition);
        m_Offset.BindProperty(m_Props.PositionOffset);
        m_EulerAngle.BindProperty(m_Props.EulerAngle);
    }

    public void DefineFields()
    {
        m_Root = new VisualElement() { name = nameof(DoorData) + "_Root" };
        m_RelativePosition = new PropertyField(m_Props.RelativePosition) { label = "Position" };
        m_Offset = new PropertyField(m_Props.PositionOffset) { label = "Offset" };
        m_EulerAngle = new PropertyField(m_Props.EulerAngle) { label = "Euler Angle" };
    }

    public void Initialize(SerializedProperty data)
    {
        m_Props = new TransformDataSerializedProperties(data);
        TransformData current = data.GetUnderlyingValue() as TransformData;
        m_PreviousData = current.Clone() as TransformData;
    }

    public void RegisterValueChangeCallbacks()
    {
        m_RelativePosition.RegisterValueChangeCallback(evt =>
        {
            RelativePosition relativePosition = evt.changedProperty.GetEnumValue<RelativePosition>();

            if (relativePosition == m_PreviousData.RelativePosition)
                return;

            m_PreviousData.RelativePosition = relativePosition;
        });
        m_Offset.RegisterValueChangeCallback(evt =>
        {
            Vector3 offset = evt.changedProperty.vector3Value;

            if (offset == m_PreviousData.PositionOffset)
                return;

            m_PreviousData.PositionOffset = offset;
        });
        m_EulerAngle.RegisterValueChangeCallback(evt =>
        {
            Vector3 euler = evt.changedProperty.vector3Value;

            if (euler == m_PreviousData.EulerAngle)
                return;

            m_PreviousData.EulerAngle = euler;
        });
    }
}