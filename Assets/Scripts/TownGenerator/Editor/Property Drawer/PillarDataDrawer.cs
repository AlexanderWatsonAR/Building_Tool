using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(PillarData))]
public class PillarDataDrawer : PropertyDrawer, IFieldInitializer
{
    VisualElement m_Root;
    PillarDataSerializedProperties m_Props;
    PillarData m_PreviousData;
    IBuildable m_Buildable;

    PropertyField m_Height, m_Width, m_Depth, m_Sides, m_IsSmooth;

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
        m_Root = new VisualElement() { name = nameof(PillarData) + "_Root"};
        m_Props = new PillarDataSerializedProperties(data);
        m_Buildable = data.serializedObject.targetObject as IBuildable;
        m_PreviousData = new PillarData(data.GetUnderlyingValue() as PillarData);
    }

    public void DefineFields()
    {
        m_Height = new PropertyField(m_Props.Height);
        m_Width = new PropertyField(m_Props.Width);
        m_Depth = new PropertyField(m_Props.Depth);
        m_Sides = new PropertyField(m_Props.Sides);
        m_IsSmooth = new PropertyField(m_Props.IsSmooth);
    }
    public void BindFields()
    {
        m_Height.BindProperty(m_Props.Height);
        m_Width.BindProperty(m_Props.Width);
        m_Depth.BindProperty(m_Props.Depth);
        m_Sides.BindProperty(m_Props.Sides);
        m_IsSmooth.BindProperty(m_Props.IsSmooth);
    }

    public void RegisterValueChangeCallbacks()
    {
        m_Height.RegisterValueChangeCallback(evt =>
        {
            float height = evt.changedProperty.floatValue;

            if (height == m_PreviousData.Height)
                return;

            m_PreviousData.Height = height;

            Build();
        });
        m_Depth.RegisterValueChangeCallback(evt =>
        {
            float depth = evt.changedProperty.floatValue;

            if (depth == m_PreviousData.Depth)
                return;

            m_PreviousData.Depth = depth;

            Build();
        });
        m_Width.RegisterValueChangeCallback(evt =>
        {
            float width = evt.changedProperty.floatValue;

            if (width == m_PreviousData.Width)
                return;

            m_PreviousData.Width = width;

            Build();
        });
        m_Sides.RegisterValueChangeCallback(evt =>
        {
            int sides = evt.changedProperty.intValue;

            if (sides == m_PreviousData.Depth)
                return;

            m_PreviousData.Sides = sides;

            Build();
        });
        m_IsSmooth.RegisterValueChangeCallback(evt =>
        {
            bool isSmooth = evt.changedProperty.boolValue;

            if (isSmooth == m_PreviousData.IsSmooth)
                return;

            m_PreviousData.IsSmooth = isSmooth;

            Build();
        });
    }

    public void AddFieldsToRoot()
    {
        m_Root.Add(m_Height);
        m_Root.Add(m_Width);
        m_Root.Add(m_Depth);
        m_Root.Add(m_Sides);
        m_Root.Add(m_IsSmooth);
    }

    public PillarData[] GetPillarDataFromBuildable()
    {
        PillarData[] data = null;

        switch(m_Buildable)
        {
            case Pillar:
                data = new PillarData[1];
                data[0] = m_Props.Data.GetUnderlyingValue() as PillarData;
                break;
        }

        return data;
    }

    public void Build()
    {
        switch(m_Buildable)
        {
            case Pillar:
                m_Buildable.Build();
                break;
        }
    }

}